using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
