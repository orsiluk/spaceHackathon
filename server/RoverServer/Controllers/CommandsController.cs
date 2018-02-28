using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RoverServer.Controllers
{
    public enum CommandType
    {
        Forward,
        Backward
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
    }

    public class CommandsController : ApiController
    {
        private List<Command> GetCommands()
        {
            object commands;
            GlobalConfiguration.Configuration.Properties.TryGetValue("CommandList", out commands);
            return (List<Command>) commands;
        }

        private bool IssueCommand(Command command)
        {
            GlobalConfiguration.Configuration.Properties.TryGetValue("CommandList", out object commandListObj);
            var commandList = (List<Command>) commandListObj;
            GlobalConfiguration.Configuration.Properties.TryGetValue("CommandQueue", out object commandQueueObj);
            var commandQueue = (ConcurrentQueue<Command>) commandQueueObj;

            if (commandList.Exists(cmd => command.Id == cmd.Id))
            {
                return false;
            }

            commandQueue.Enqueue(command);
            commandList.Add(command);
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
    }
}