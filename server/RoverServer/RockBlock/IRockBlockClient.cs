using RoverServer.Controllers;

namespace RoverServer.RockBlock
{
    public class RockBlockResult
    {
        public int? ErrorCode { get; set; }
        public string Description { get; set; }
        public int? MessageId { get; set; }

        public override string ToString()
        {
            if (ErrorCode != null)
            {
                return $"RockBlock Error {ErrorCode}: {Description}";
            }
            return $"RockBlock Success: {MessageId}";
        }
    }

    interface IRockBlockClient
    {
        RockBlockResult SendMessage(string data);
        RockBlockResult SendCommand(Command data);
    }
}