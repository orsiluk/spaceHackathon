using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Text;
using System.Web.Http;
using System.Web.Http.Results;
using FluentScheduler;
using Microsoft.ApplicationInsights;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RoverServer.RockBlock;

namespace RoverServer.Controllers
{
    public class Message
    {
        public string imei { get; set; }
        public string momsn { get; set; }
        public string transmit_time { get; set; }
        public float iridium_latitude { get; set; }
        public float iridium_longitude { get; set; }
        public float iridium_cep { get; set; }
        public string data { get; set; }
    }

    public class ReceiveController : ApiController
    {
        public static byte[] FromHex(string hex)
        {
            hex = hex.Replace("-", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }

        // POST api/receive/message
        [System.Web.Http.HttpPost]
        public IHttpActionResult Message([FromBody] Message data)
        {
            var bytes = FromHex(data.data);
            var message = Encoding.ASCII.GetString(bytes);

            var telemetry = new TelemetryClient();
            telemetry.TrackEvent($"Received message: {message}");

            GlobalConfiguration.Configuration.Properties["RobotMessage"] = message;

            return Content(HttpStatusCode.OK, "Success");
        }

        // GET api/receive/get
        public string Get()
        {
            return "Hello world";
        }
    }
}