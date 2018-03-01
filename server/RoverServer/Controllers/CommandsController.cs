using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Results;
using FluentScheduler;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RoverServer.RockBlock;

namespace RoverServer.Controllers
{
    public enum CommandType
    {
        Forward,
        Backward,
        Left,
        Right
    }

    public class Command
    {
        [JsonProperty(Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(Required = Required.Always)]
        [JsonConverter(typeof(StringEnumConverter))]
        public CommandType CommandType { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Data { get; set; }

        public override string ToString()
        {
            return $"Command {Id} : {CommandType} | {Data}";
        }

        public bool IsValid()
        {
            return Data != null;
        }
    }

    public class CommandsController : ApiController
    {
        private IRockBlockClient GetRockBlockClient()
        {
            GlobalConfiguration.Configuration.Properties.TryGetValue("RockBlockClient", out object rockBlockClient);
            return (IRockBlockClient) rockBlockClient;
        }

        private List<Command> GetCommands()
        {
            GlobalConfiguration.Configuration.Properties.TryGetValue("CommandList", out object commands);
            return (List<Command>) commands;
        }

        private void ResetCommands()
        {
            GlobalConfiguration.Configuration.Properties.TryGetValue("CommandList", out object objCommands);
            var commands = (List<Command>)objCommands;
            commands.RemoveAll(p => true);
        }

        private bool IssueCommand(Command command)
        {
            GlobalConfiguration.Configuration.Properties.TryGetValue("CommandList", out object commandListObj);
            var commandList = (List<Command>) commandListObj;

            var telemetry = new Microsoft.ApplicationInsights.TelemetryClient();

            if (!command.IsValid())
            {
                return false;
            }

            if (commandList.Exists(cmd => command.Id == cmd.Id))
            {
                return false;
            }
            
            commandList.Add(command);
            JobManager.AddJob(() =>
            {
                if (Robot.Mode == Robot.CommsMode.NXT)
                {
                    telemetry.TrackEvent("Sending message to NXT");
                    Robot.HandleCommand(command);
                }
                else
                {
                    telemetry.TrackEvent("Sending message to RockBlock");
                    var response = GetRockBlockClient().SendCommand(command);
                    telemetry.TrackEvent($"Response from RockBlock: {response}");
                }
            }, (s) => s.ToRunOnceIn(0).Seconds());
            return true;
        }

        // GET api/commands/getall
        public JsonResult<List<Command>> GetAll()
        {
            return Json(GetCommands());
        }

        // GET api/commands/5
        public JsonResult<Command> Get(int id)
        {
            return Json(GetCommands().First(cmd => cmd.Id == id));
        }

        // POST api/commands/issue
        [HttpPost]
        [ActionName("Issue")]
        public IHttpActionResult Issue([FromBody] Command value)
        {
            return IssueCommand(value)
                ? Content(HttpStatusCode.OK, $"Successfully issue command {value.Id}")
                : Content(HttpStatusCode.BadRequest, "Failed to issue command");
        }

        [HttpDelete]
        [ActionName("Reset")]
        public IHttpActionResult Reset()
        {
            ResetCommands();
            return Content(HttpStatusCode.OK, "Successfully reset command history");
        }
    }
}