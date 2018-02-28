using RoverServer.Controllers;

namespace RoverServer.RockBlock
{
    public class RockBlockResult
    {
        public int? ErrorCode { get; set; }
        public string Description { get; set; }
        public int? MessageId { get; set; }
    }

    interface IRockBlockClient
    {
        RockBlockResult SendMessage(string data);
        RockBlockResult SendCommand(Command data);
    }
}
