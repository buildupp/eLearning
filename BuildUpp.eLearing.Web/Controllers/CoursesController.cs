using BuildUpp.eLearing.Web.Models;
using BuildUpp.eLearning.Data;
using BuildUpp.eLearning.Data.Repositories;
using BuildUpp.eLearning.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;

namespace BuildUpp.eLearing.Web.Controllers
{
    public class CoursesController : BaseApiController
    {
        public CoursesController(ILearningRepository repo)
            : base(repo)
        {
        }

        public IEnumerable<CourseModel> Get()
        {
            IQueryable<Course> query;

            query = Repository.GetAllCourses();

            var results = query.ToList().Select(s => TheModelFactory.Create(s));

            return results;
        }

        //public Object Get(int page = 0, int pageSize = 10)
        //{
        //    IQueryable<Course> query;

        //    query = Repository.GetAllCourses().OrderBy(c => c.CourseSubject.Id);
        //    var totalCount = query.Count();
        //    var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        //    var urlHelper = new UrlHelper(Request);
        //    var prevLink = page > 0 ? urlHelper.Link("Courses", new { page = page - 1 }) : "";
        //    var nextLink = page < totalPages - 1 ? urlHelper.Link("Courses", new { page = page + 1 }) : "";

        //    var results = query
        //    .Skip(pageSize * page)
        //    .Take(pageSize)
        //    .ToList()
        //    .Select(s => TheModelFactory.Create(s));

        //    return new
        //    {
        //        TotalCount = totalCount,
        //        TotalPages = totalPages,
        //        PrevPageLink = prevLink,
        //        NextPageLink = nextLink,
        //        Results = results
        //    };

        //}

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

        public HttpResponseMessage GetCourse(int id)
        {
            try
            {
                var course = Repository.GetCourse(id);
                if (course != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, TheModelFactory.Create(course));
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

        public HttpResponseMessage Post([FromBody] CourseModel courseModel)
        {
            try
            {
                var entity = TheModelFactory.Parse(courseModel);

                if (entity == null) Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not read subject/tutor from body");

                if (Repository.Insert(entity) && Repository.SaveAll())
                {
                    return Request.CreateResponse(HttpStatusCode.Created, TheModelFactory.Create(entity));
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
        public HttpResponseMessage Put(int id, [FromBody] CourseModel courseModel)
        {
            try
            {

                var updatedCourse = TheModelFactory.Parse(courseModel);

                if (updatedCourse == null) Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not read subject/tutor from body");

                var originalCourse = Repository.GetCourse(id, false);

                if (originalCourse == null || originalCourse.Id != id)
                {
                    return Request.CreateResponse(HttpStatusCode.NotModified, "Course is not found");
                }
                else
                {
                    updatedCourse.Id = id;
                }

                if (Repository.Update(originalCourse, updatedCourse) && Repository.SaveAll())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, TheModelFactory.Create(updatedCourse));
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

        public HttpResponseMessage Delete(int id)
        {
            try
            {
                var course = Repository.GetCourse(id);

                if (course == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                if (course.Enrollments.Count > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Can not delete course, students has enrollments in course.");
                }

                if (Repository.DeleteCourse(id) && Repository.SaveAll())
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
