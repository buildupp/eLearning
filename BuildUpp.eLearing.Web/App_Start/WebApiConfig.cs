using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace BuildUpp.eLearing.Web.App_Start
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                    name: "Courses",
                    routeTemplate: "api/courses/{id}",
                    defaults: new { controller = "courses", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                    name: "Students",
                    routeTemplate: "api/students/{userName}",
                    defaults: new { controller = "students", userName = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                    name: "Enrollments",
                    routeTemplate: "api/courses/{courseId}/students/{userName}",
                    defaults: new { controller = "Enrollments", userName = RouteParameter.Optional }
            );

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
        }
    }
}