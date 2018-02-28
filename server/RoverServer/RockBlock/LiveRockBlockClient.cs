using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
            _telemetry = new TelemetryClient();
        }

        private string ToString(Command command)
        {
            var str =
                $"B{command.Id.ToStringInvariant()}:{command.CommandType.ToString().ToUpperInvariant()}:{command.Data.ToUpperInvariant()}";
            byte[] bytes = Encoding.ASCII.GetBytes(str);
            var hexString = BitConverter.ToString(bytes);
            return hexString.Replace("-", "");
        }

        private RockBlockResult DecodeResponse(HttpResponseMessage response)
        {
            var messageSegments = response.Content.ReadAsStringAsync().Result.Split(',');
            if (messageSegments.Length < 1)
            {
                throw new HttpRequestException("Empty response from RockBlock");
            }
            switch (messageSegments[0].ToUpperInvariant())
            {
                case "OK":
                    if (int.TryParse(messageSegments[1], out int messageId))
                    {
                        return new RockBlockResult {MessageId = messageId};
                    }
                    throw new HttpRequestException("Non-integer message ID from RockBlock");
                case "FAILED":
                    if (int.TryParse(messageSegments[1], out int errorId))
                    {
                        return new RockBlockResult {Description = messageSegments[2], ErrorCode = errorId};
                    }
                    throw new HttpRequestException($"Invalid error code {messageSegments[1]} from RockBlock");
            }
            throw new HttpRequestException("Invalid response from RockBlock");
        }

        public RockBlockResult SendMessage(string data)
        {
            var parameters = new Dictionary<string, string>
            {
                {"imei", _imei},
                {"username", _username},
                {"password", _password},
                {"data", data}
            };
            var encodedParameters = new FormUrlEncodedContent(parameters);
            var task = Task.Run(async () =>
            {
                using (var client = new HttpClient())
                {
                    return await client.PostAsync(_baseUri, encodedParameters);
                }
            });
            var result = task.Result;
            return DecodeResponse(result);
        }

        public RockBlockResult SendCommand(Command command)
        {
            _telemetry.TrackEvent($"Hexified message: {ToString(command)}");
            return SendMessage(ToString(command));
        }
    }
}