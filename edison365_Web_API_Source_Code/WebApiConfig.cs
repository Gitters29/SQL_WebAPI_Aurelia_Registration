using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Web.Http.Routing;
using System.Net.Http;

namespace TMSWebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Enable Cross-Origin Resource Sharing
            config.EnableCors();

            // Web API configuration and services
            config.Formatters.JsonFormatter.SupportedMediaTypes
            .Add(new MediaTypeHeaderValue("text/html"));
            // Web API routes
            config.MapHttpAttributeRoutes();

            // This enables us to have multiple Get/Post/Put/Delete methods in
            // our UserController.
            config.Routes.MapHttpRoute(
                "DefaultApiWithId", 
                "api/{controller}/{id}", 
                new { id = RouteParameter.Optional }, 
                new { id = @"\d+" });
            config.Routes.MapHttpRoute(
                "DefaultApiWithAction", 
                "api/{controller}/{action}");
            config.Routes.MapHttpRoute(
                "DefaultApiWithActionAndId",
                "api/{controller}/{action}/{id}",
                new { id = RouteParameter.Optional }, 
                new { id = @"\d+" });
            config.Routes.MapHttpRoute(
                "DefaultApiGet", 
                "api/{controller}", 
                new { action = "Get" }, 
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute(
                "DefaultApiPost", 
                "api/{controller}", 
                new { action = "Post" }, 
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

        }
    }
}
