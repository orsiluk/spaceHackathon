using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;

namespace sat_comms
{
    class ISU
    {
        private SerialPort port;

        public ISU(string COMPort)
        {
            port = new SerialPort(COMPort, 19200, Parity.None, 8, StopBits.One);
            port.Handshake = Handshake.RequestToSend;
            port.NewLine = "\n";
            port.Encoding = Encoding.ASCII;
            port.DiscardNull = false;
            port.ReadTimeout = 1000;
        }

        public void Open()
        {
            if (!port.IsOpen)
            {
                port.Open();
                readResponse();
            }
        }

        public void Close()
        {
            if (port.IsOpen)
            {
                port.Close();
            }
        }

        private void issueExtendedATCommand(string cmd, string prefix = "")
        {
            string cmdString = "AT" + (prefix != "" ? "+" + prefix : "") + cmd;
            Console.WriteLine("Issuing command: " + cmdString);
            port.WriteLine(cmdString);
        }

        private Tuple<List<string>, bool> readResponse()
        {
            string line = "";
            while (!line.EndsWith("OK"))
            {
                try
                {
                    int i = port.ReadByte();
                    char c = (char)i;
                    Console.Write(c);
                    line += c;
                }
                catch(TimeoutException)
                {
                    // Do nothing
                    // TODO
                }
            }
            Console.Write('\n');
            List<string> lines = line.Split("\n".ToCharArray()).ToList();
            return new Tuple<List<string>, bool>(lines, lines[lines.Count-1].StartsWith("OK"));
        }

        public string ISN
        {
            get
            {
                issueExtendedATCommand("SN", "G");
                Tuple<List<string>, bool> response = readResponse();
                if (response.Item2)
                {
                    throw new Exception("Error reading ISN (IMEI) number. Data:\n" + String.Join("\n", response.Item1));
                }
                else if (response.Item1.Count != 3)
                {
                    throw new Exception("Error reading ISN (IMEI) number. Incorrect number of data items. Data:\n" + String.Join("\n", response.Item1));
                }
                else
                {
                    return response.Item1[0];
                }
            }
        }
    }
}
