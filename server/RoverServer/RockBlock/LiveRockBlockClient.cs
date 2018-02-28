using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.Ajax.Utilities;
using Microsoft.ApplicationInsights;
using RoverServer.Controllers;

namespace RoverServer.RockBlock
{
    public class LiveRockBlockClient : IRockBlockClient
    {
        private string _imei;
        private string _username;
        private string _password;
        private Uri _baseUri = new Uri("https://core.rock7.com/rockblock/MT");
        private TelemetryClient _telemetry;

        public LiveRockBlockClient(string imei, string username, string password)
        {
            _imei = imei;
            _username = username;
            _password = password;
            _telemetry = new Microsoft.ApplicationInsights.TelemetryClient();
        }

        private string ToString(Command command)
        {
            var str = $"B{command.CommandType.ToString().ToUpperInvariant()}:{command.Data.ToString().ToUpperInvariant()}";
            byte[] bytes = Encoding.ASCII.GetBytes(str);
            var hexString = BitConverter.ToString(bytes);
            return hexString.Replace("-", "");
        }

        public RockBlockResult SendMessage(string data)
        {
            return new RockBlockResult {ErrorCode = null, Description = "Okay!", MessageId = 0};
        }

        public RockBlockResult SendCommand(Command command)
        {
            _telemetry.TrackEvent($"Hexified message: {ToString(command)}");
            return SendMessage(ToString(command));
        }
    }
}