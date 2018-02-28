using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;
using RoverServer.Controllers;

namespace RoverServer.RockBlock
{
    public class LiveRockBlockClient : IRockBlockClient
    {
        private string _imei;
        private string _username;
        private string _password;
        private Uri _baseUri = new Uri("https://core.rock7.com/rockblock/MT");

        public LiveRockBlockClient(string imei, string username, string password)
        {
            _imei = imei;
            _username = username;
            _password = password;
        }

        public RockBlockResult SendMessage(string data)
        {
            return new RockBlockResult {ErrorCode = null, Description = "Okay!", MessageId = 0};
        }

        public RockBlockResult SendCommand(Command data)
        {
            return new RockBlockResult {ErrorCode = null, Description = "Okay!", MessageId = 0};
        }
    }
}