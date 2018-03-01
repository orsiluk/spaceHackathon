using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FluentScheduler;
using NSwag.AspNet.Owin;
using RoverServer.Controllers;
using NKH.MindSqualls;
using RoverServer.RockBlock;

namespace RoverServer
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            JobManager.Initialize(new Registry());
            RouteTable.Routes.MapOwinPath("swagger", app =>
            {
                app.UseSwaggerUi(typeof(WebApiApplication).Assembly, new SwaggerUiSettings
                {
                    MiddlewareBasePath = "/swagger"
                });
            });

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            new Thread(() =>
            {
                var telemetry = new Microsoft.ApplicationInsights.TelemetryClient();
                GlobalConfiguration.Configuration.Properties.TryGetValue("CommandQueue", out object objCommands);
                GlobalConfiguration.Configuration.Properties.TryGetValue("RockBlockClient", out object objRockBlockClient);
                var rockBlockClient = (IRockBlockClient)objRockBlockClient;
                var commands = (ConcurrentQueue<Command>)objCommands;
                while (true)
                {
                    if (commands.TryDequeue(out Command cmd))
                    {
                        if (Robot.Mode == RobotLib.Robot.CommsMode.NXT)
                        {
                            telemetry.TrackEvent("Sending message to NXT");
                            Robot.HandleCommand(cmd);
                        }
                        else
                        {
                            telemetry.TrackEvent("Sending message to RockBlock");
                            var response = rockBlockClient.SendCommand(cmd);
                            telemetry.TrackEvent($"Response from RockBlock: {response}");
                        }
                    }

                    Thread.Sleep(1000);
                }
            }).Start();
        }

        public override void Dispose()
        {
            Robot.Cleanup();

            base.Dispose();
        }
    }
}
