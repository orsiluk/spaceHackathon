using RoverServer.Controllers;

namespace RoverServer.RockBlock
{
    public class MockRockBlockClient : IRockBlockClient
    {
        public MockRockBlockClient()
        {
        }

        public RockBlockResult SendMessage(string data)
        {
            return new RockBlockResult {ErrorCode = null, Description = "Okay!", MessageId = 0};
        }

        public RockBlockResult SendCommand(Command data)
        {
            return SendMessage("");
        }
    }
}