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
using RoverServer.Filters;
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

        [JsonProperty(Required = Required.Always)]
        public int AutomaticId { get; set; }

        public override string ToString()
        {
            return $"Command {Id} (auto: {AutomaticId}) : {CommandType} | {Data}";
        }

        public bool IsValid()
        {
            return Data != null;
        }
    }

    [InterceptorFilter]
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

        private ConcurrentQueue<Command> GetCommandQueue()
        {
            GlobalConfiguration.Configuration.Properties.TryGetValue("CommandQueue", out object commands);
            return (ConcurrentQueue<Command>) commands;
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

            if (commandList.Exists(cmd => command.AutomaticId == cmd.AutomaticId))
            {
                return false;
            }

            //commandList.Add(command);
            var commandQueue = GetCommandQueue();
            commandQueue.Enqueue(command);

            return true;
        }

        // GET api/commands/GetAll
        public JsonResult<List<Command>> GetAll()
        {
            return Json(GetCommands());
        }

        // GET api/commands/GetById/5
        public JsonResult<Command> GetById(int id)
        {
            return Json(GetCommands().FirstOrDefault(cmd => cmd.Id == id));
        }

        // GET api/commands/GetByAutomaticId/5
        public JsonResult<Command> GetByAutomaticId(int id)
        {
            return Json(GetCommands().FirstOrDefault(cmd => cmd.AutomaticId == id));
        }

        // POST api/commands/issue
        [HttpPost]
        [ActionName("Issue")]
        public IHttpActionResult Issue([FromBody] Command value)
        {
            GlobalConfiguration.Configuration.Properties.TryGetValue("MessageId", out object messageId);
            GlobalConfiguration.Configuration.Properties.TryUpdate("MessageId", (int)messageId + 1, (int)messageId);
            value.AutomaticId = (int)messageId;

            return IssueCommand(value)
                ? Content(HttpStatusCode.OK, $"Successfully issue command {value.Id}:{value.CommandType}:{value.Data}")
                : Content(HttpStatusCode.BadRequest, $"Failed to issue command {value.Id}:{value.CommandType}:{value.Data}");
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