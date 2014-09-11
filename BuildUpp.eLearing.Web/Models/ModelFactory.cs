using BuildUpp.eLearning.Data.Repositories;
using BuildUpp.eLearning.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Routing;

namespace BuildUpp.eLearing.Web.Models
{
    public class ModelFactory
    {
        private UrlHelper _UrlHelper;
        private ILearningRepository _repository;

        public ModelFactory(HttpRequestMessage request, ILearningRepository repository)
        {
            _UrlHelper = new UrlHelper(request);
            _repository = repository;
        }

        public CourseModel Create(Course course)
        {
            return new CourseModel()
            {
                Url = _UrlHelper.Link("Courses", new { id = course.Id }),
                Id = course.Id,
                Name = course.Name,
                Duration = course.Duration,
                Description = course.Description,
                Tutor = Create(course.CourseTutor),
                Subject = Create(course.CourseSubject)
            };
        }

        public TutorModel Create(Tutor tutor)
        {
            return new TutorModel()
            {
                Id = tutor.Id,
                Email = tutor.Email,
                UserName = tutor.UserName,
                FirstName = tutor.FirstName,
                LastName = tutor.LastName,
                Gender = tutor.Gender
            };
        }

        public SubjectModel Create(Subject subject)
        {
            return new SubjectModel()
            {
                Id = subject.Id,
                Name = subject.Name
            };
        }

        public EnrollmentModel Create(Enrollment enrollment)
        {
            return new EnrollmentModel()
            {
                EnrollmentDate = enrollment.EnrollmentDate,
                Course = Create(enrollment.Course)
            };
        }

        public StudentModel Create(Student student)
        {
            return new StudentModel()
            {
                Url = _UrlHelper.Link("Students", new { userName = student.UserName }),
                Id = student.Id,
                Email = student.Email,
                UserName = student.UserName,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Gender = student.Gender,
                DateOfBirth = student.DateOfBirth,
                RegistrationDate = student.RegistrationDate.Value,
                EnrollmentsCount = student.Enrollments.Count(),
                Enrollments = student.Enrollments.Select(e => Create(e))
            };
        }

        public Course Parse(CourseModel model)
        {
            try
            {
                var course = new Course()
                {
                    Name = model.Name,
                    Description = model.Description,
                    Duration = model.Duration,
                    CourseSubject = _repository.GetSubject(model.Subject.Id),
                    CourseTutor = _repository.GetTutor(model.Tutor.Id)
                };

                return course;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public StudentBaseModel CreateSummary(Student student)
        {
            return new StudentBaseModel()
            {
                Url = _UrlHelper.Link("Students", new { userName = student.UserName }),
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Gender = student.Gender,
                EnrollmentsCount = student.Enrollments.Count(),
            };
        }
    }
}