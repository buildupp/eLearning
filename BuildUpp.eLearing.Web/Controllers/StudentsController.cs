using BuildUpp.eLearing.Web.Filters;
using BuildUpp.eLearing.Web.Models;
using BuildUpp.eLearning.Data.Repositories;
using BuildUpp.eLearning.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;

namespace BuildUpp.eLearing.Web.Controllers
{
    public class StudentsController : BaseApiController
    {
        public StudentsController(ILearningRepository repository)
            : base(repository)
        {
        }

        public IEnumerable<StudentBaseModel> Get(int page = 0, int pageSize = 10)
        {
            IQueryable<Student> query;

            query = Repository.GetAllStudentsWithEnrollments().OrderBy(c => c.LastName);

            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var urlHelper = new UrlHelper(Request);
            var prevLink = page > 0 ? urlHelper.Link("Students", new { page = page - 1, pageSize = pageSize }) : "";
            var nextLink = page < totalPages - 1 ? urlHelper.Link("Students", new { page = page + 1, pageSize = pageSize }) : "";

            var paginationHeader = new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                PrevPageLink = prevLink,
                NextPageLink = nextLink
            };

            System.Web.HttpContext.Current.Response.Headers.Add("X-Pagination",
                                                                Newtonsoft.Json.JsonConvert.SerializeObject(paginationHeader));

            var results = query
                        .Skip(pageSize * page)
                        .Take(pageSize)
                        .ToList()
                        .Select(s => TheModelFactory.CreateSummary(s));

            return results;
        }

        [LearningAuthorizeAttribute]
        public HttpResponseMessage Get(string userName)
        {
            try
            {
                var student = Repository.GetStudentEnrollments(userName);
                if (student != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, TheModelFactory.Create(student));
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

        }

        public HttpResponseMessage Post([FromBody] Student student)
        {
            try
            {
                if (Repository.Insert(student) && Repository.SaveAll())
                {
                    return Request.CreateResponse(HttpStatusCode.Created, TheModelFactory.Create(student));
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not save to the database.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

        }

        [HttpPatch]
        [HttpPut]
        public HttpResponseMessage Put(string userName, [FromBody] Student student)
        {
            try
            {

                var originalStudent = Repository.GetStudent(userName);

                if (originalStudent == null || originalStudent.UserName != userName)
                {
                    return Request.CreateResponse(HttpStatusCode.NotModified, "Studnet is not found");
                }
                else
                {
                    student.Id = originalStudent.Id;
                }

                if (Repository.Update(originalStudent, student) && Repository.SaveAll())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, TheModelFactory.Create(student));
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotModified);
                }

            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public HttpResponseMessage Delete(string userName)
        {
            try
            {
                var student = Repository.GetStudent(userName);

                if (student == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                if (student.Enrollments.Count > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Can not delete student, student has enrollments in courses.");
                }

                if (Repository.DeleteStudent(student.Id) && Repository.SaveAll())
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

    }
}