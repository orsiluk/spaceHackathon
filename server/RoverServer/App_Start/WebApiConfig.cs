using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using NSwag.SwaggerGeneration.WebApi;
using RoverServer.Controllers;
using RoverServer.RockBlock;

//using RoverServer.RockBlock;

namespace RoverServer
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var settings = new WebApiToSwaggerGeneratorSettings
            {
                DefaultUrlTemplate = "api/{controller}/{action}/{id}"
            };

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new {id = RouteParameter.Optional}
            );

            /*config.Properties.TryAdd("RockBlockClient",
                new LiveRockBlockClient(ConfigurationManager.AppSettings["rockblockIMEI"],
                    ConfigurationManager.AppSettings["rockblockUsername"],
                    ConfigurationManager.AppSettings["rockblockPassword"]));*/
            config.Properties.TryAdd("RockBlockClient", new MockRockBlockClient());
            config.Properties.TryAdd("CommandList", new List<Command>());

            Robot.Init();
        }
    }
}