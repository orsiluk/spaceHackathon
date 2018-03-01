using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace sat_comms
{
    class ISU : IDisposable
    {
        private SerialPort port;

        public enum ExtendedCommandGroups
        {
            None,
            Cellular,
            DataCompression,
            Generic,
            InterfaceControl,
            ShortBurstData,
            MotorolaSatelliteProductProprietary
        }

        public readonly string[] ExtendedCommandGroupPrefixes =
        {
            "",
            "+C",
            "+D",
            "+G",
            "+I",
            "+SBD",
            "-MS",
        };

        public enum Verbosities
        {
            Unset = -1,
            Numeric = 0,
            Verbose = 1
        }

        public ISU(string COMPort)
        {
            port = new SerialPort(COMPort, 19200, Parity.None, 8, StopBits.One);
            port.Handshake = Handshake.RequestToSend;
            port.NewLine = "\r\n";
            port.Encoding = Encoding.ASCII;
            port.DiscardNull = false;
            //port.ReadTimeout = 1000;
            //port.DataReceived += Port_DataReceived;
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = port.ReadExisting();
                Console.WriteLine("Received data: " + data);
            }
            catch
            {
                // TODO
            }
        }

        public void Open()
        {
            if (!port.IsOpen)
            {
                port.Open();
            }
        }

        public void Close()
        {
            if (port.IsOpen)
            {
                port.Close();
            }
        }

        private void issueBasicATCommand(string cmd, int? param = null)
        {
            cmd = cmd.ToUpper();
            string cmdString = "AT" + cmd + (param.HasValue ? param.ToString() : "");
            //Console.WriteLine("Issuing basic command: " + cmdString);
            port.WriteLine(cmdString);
            port.BaseStream.Flush();
        }

        private void issueExtendedATCommand(string cmd, ExtendedCommandGroups prefix = ExtendedCommandGroups.None,
            int[] args = null)
        {
            cmd = cmd.ToUpper();
            string cmdString = "AT" + ExtendedCommandGroupPrefixes[(int) prefix] + cmd;
            if (args != null)
            {
                cmdString += "=";
                for (int i = 0; i < args.Length; i++)
                {
                    cmdString += args[i].ToString();
                    if (i < args.Length - 1)
                    {
                        cmdString += ",";
                    }
                }
            }
            //Console.WriteLine("Issuing extended command: " + cmdString);
            port.WriteLine(cmdString);
            port.BaseStream.Flush();
        }

        private void abortCommand()
        {
            port.Write("\x30");
        }

        private void repeatLastCommand()
        {
            port.Write("A/"); //Spec: Don't follow with <CR>
        }

        private Tuple<List<string>, bool> readResponse()
        {
            string line = "";
            List<string> lines = new List<string>();
            do
            {
                line = port.ReadTo("\r").Replace("\n", "");
                lines.Add(line);
            } while (!line.StartsWith("OK") && !line.StartsWith("ERROR") && !line.StartsWith("READY"));

            lines.RemoveAll((x) => String.IsNullOrWhiteSpace(x));
            return new Tuple<List<string>, bool>(lines,
                lines[lines.Count - 1].StartsWith("OK") || lines[lines.Count - 1].StartsWith("READY"));
        }

        private void ThrowFailedResponse(string message, Tuple<List<string>, bool> response)
        {
            throw new Exception(message + "\r\nData:\r\n" + String.Join("\r\n", response.Item1));
        }

        private Verbosities _verbosity = Verbosities.Unset;

        public Verbosities Verbosity
        {
            get { return _verbosity; }
            set
            {
                _verbosity = value;
                issueBasicATCommand("V", (int) value);
                if (!ResponseSuppression)
                {
                    Tuple<List<string>, bool> response = readResponse();
                    if (!response.Item2)
                    {
                        ThrowFailedResponse("Failed to set verbosity.", response);
                    }
                }
            }
        }

        private bool responseSuppression = true;

        public bool ResponseSuppression
        {
            get { return responseSuppression; }
            set
            {
                responseSuppression = value;
                issueBasicATCommand("Q", value ? 1 : 0);
                if (!value)
                {
                    Tuple<List<string>, bool> response = readResponse();
                    if (!response.Item2)
                    {
                        ThrowFailedResponse("Failed to set response suppression.", response);
                    }
                }
            }
        }

        private bool echoEnable = false;

        public bool EchoEnable
        {
            get { return echoEnable; }
            set
            {
                echoEnable = value;
                issueBasicATCommand("E", value ? 1 : 0);
                if (!ResponseSuppression)
                {
                    Tuple<List<string>, bool> response = readResponse();
                    if (!response.Item2)
                    {
                        ThrowFailedResponse("Failed to set echo enable.", response);
                    }
                }
            }
        }

        private bool radioEnable = false;

        public bool RadioEnable
        {
            get { return radioEnable; }
            set
            {
                radioEnable = value;
                issueBasicATCommand("*R", value ? 1 : 0);
                if (!ResponseSuppression)
                {
                    Tuple<List<string>, bool> response = readResponse();
                    if (!response.Item2)
                    {
                        ThrowFailedResponse("Failed to set radio enable.", response);
                    }
                }
            }
        }

        private string getValue(bool extended, string command, int? param, string name,
            ExtendedCommandGroups extGroup = ExtendedCommandGroups.None)
        {
            return getValues(extended, command, param, name, extGroup)[0];
        }

        private List<string> getValues(bool extended, string command, int? param, string name,
            ExtendedCommandGroups extGroup = ExtendedCommandGroups.None)
        {
            if (Verbosity != Verbosities.Verbose)
            {
                throw new Exception("Wrong verbosity mode. Require verbose mode.");
            }
            if (ResponseSuppression)
            {
                throw new Exception("Wrong response suppression mode. Require no suppression.");
            }

            if (extended)
            {
                issueExtendedATCommand(command, extGroup, param.HasValue ? new int[] {param.Value} : null);
            }
            else
            {
                issueBasicATCommand(command, param);
            }
            Tuple<List<string>, bool> response = readResponse();
            if (!response.Item2)
            {
                ThrowFailedResponse("Error reading " + name + ".", response);
                return null;
            }
            else
            {
                int start = EchoEnable ? 1 : 0;
                int count = EchoEnable ? response.Item1.Count - 2 : response.Item1.Count - 1;
                return response.Item1.GetRange(start, count);
            }
        }

        public string SoftwareRevision
        {
            get { return getValue(false, "I", 3, "software revision"); }
        }

        public string ProductFamily
        {
            get { return getValue(false, "I", 4, "product family"); }
        }

        public string HardwareSpecification
        {
            get { return getValue(false, "I", 7, "hardware specification"); }
        }

        public string FactoryIdentity
        {
            get { return getValue(false, "I", 6, "factory identity"); }
        }

        public string ISN
        {
            get { return getValue(true, "SN", null, "ISN (IMEI)", ExtendedCommandGroups.Generic); }
        }

        public string AllRegisters
        {
            get { return String.Join("\n", getValues(false, "%R", null, "all registers")); }
        }

        public int SignalQuality
        {
            get
            {
                string result =
                    getValue(true, "SQ", null, "signal quality", ExtendedCommandGroups.Cellular)
                        .Split(":".ToCharArray())[1];
                return int.Parse(result);
            }
        }

        public void WriteMessageData(byte[] data)
        {
            if (data.Length > 340)
            {
                throw new OutOfMemoryException("Maximum mobile-originated data length is 340 bytes.");
            }
            if (data.Length < 1)
            {
                throw new OutOfMemoryException("Minimum mobile-originated data length is 1 byte.");
            }

            issueExtendedATCommand("WB", ExtendedCommandGroups.ShortBurstData, new int[] {data.Length});
            bool isReady = readResponse().Item2;
            if (!isReady)
            {
                throw new Exception("Device did not report ready for Short Burst Data write.");
            }

            int checksum = 0;
            foreach (byte x in data)
            {
                checksum += x;
            }
            byte checksumLo = (byte) (checksum & 0xFF);
            byte checksumHi = (byte) ((checksum >> 8) & 0xFF);

            // Checksum: High byte first as per spec
            byte[] finalData = data.Concat(new byte[] {checksumHi, checksumLo}).ToArray();
            port.Write(finalData, 0, finalData.Length);

            var response = readResponse();
            if (!response.Item2)
            {
                throw new Exception("SBD Write didn't respond as expected.");
            }

            if (!response.Item1[0].StartsWith("0"))
            {
                throw new Exception("Failed to write short burst data. Error code: " + response.Item1[0]);
            }
        }

        public byte[] ReadMessageData()
        {
            issueExtendedATCommand("RB", ExtendedCommandGroups.ShortBurstData);
            byte lf1 = (byte) port.ReadByte(); // LF
            Console.Write(lf1.ToString("X2") + " ");
            for (int i = 0; i < "AT+SBDRB".Length; i++)
            {
                Console.Write(port.ReadByte().ToString("X2") + " ");
            }
            byte cr = (byte) port.ReadByte(); // CR
            byte lf2 = (byte) port.ReadByte(); // LF
            Console.Write(cr.ToString("X2") + " ");
            Console.Write(lf2.ToString("X2") + " ");

            byte lenByte1 = (byte) port.ReadByte();
            byte lenByte2 = (byte) port.ReadByte();
            Console.Write(lenByte1.ToString("X2") + " ");
            Console.Write(lenByte2.ToString("X2") + " ");

            int length = (lenByte1 << 8) + (lenByte2);
            byte[] data = new byte[length];
            int ownChecksum = 0;
            for (int i = 0; i < length; i++)
            {
                data[i] = (byte) port.ReadByte();
                Console.Write(data[i].ToString("X2") + " ");
                ownChecksum += data[i];
            }
            ownChecksum = ownChecksum & 0xFFFF;
            int checksum = (port.ReadByte() << 8) + port.ReadByte();
            if (checksum != ownChecksum)
            {
                throw new Exception("Read data checksum mismatch: " + ownChecksum + " != " + checksum);
            }

            var response = readResponse();
            if (!response.Item2)
            {
                ThrowFailedResponse("Failed to read message.", response);
            }
            return data;
        }

        public Tuple<int, int, int, int, int, int> InitiateSBDSession()
        {
            issueExtendedATCommand("IX", ExtendedCommandGroups.ShortBurstData);
            System.Threading.Thread.Sleep(1000);
            var response = readResponse();
            if (!response.Item2)
            {
                ThrowFailedResponse("Failed to initiate SBD session.", response);
            }

            string statusLine = response.Item1[1].Split(":".ToCharArray())[1];
            string[] statuses = statusLine.Split(",".ToCharArray());

            int MOStatus = int.Parse(statuses[0]);
            int MOMSN = int.Parse(statuses[1]);
            int MTStatus = int.Parse(statuses[2]);
            int MTMSN = int.Parse(statuses[3]);
            int MTLength = int.Parse(statuses[4]);
            int MTQueued = int.Parse(statuses[5]);

            return new Tuple<int, int, int, int, int, int>(MOStatus, MOMSN, MTStatus, MTMSN, MTLength, MTQueued);
        }

        public void ClearMobileOriginatedBuffer()
        {
            issueExtendedATCommand("D0", ExtendedCommandGroups.ShortBurstData);
            var response = readResponse();
            if (!response.Item2 || response.Item1[1] != "0")
            {
                ThrowFailedResponse("Failed to clear mobile-originated buffer.", response);
            }
        }

        public void ClearMobileTerminatedBuffer()
        {
            issueExtendedATCommand("D1", ExtendedCommandGroups.ShortBurstData);
            var response = readResponse();
            if (!response.Item2 || response.Item1[1] != "0")
            {
                ThrowFailedResponse("Failed to clear mobile-terminated buffer.", response);
            }
        }

        public void ClearMobileBuffers()
        {
            issueExtendedATCommand("D2", ExtendedCommandGroups.ShortBurstData);
            var response = readResponse();
            if (!response.Item2 || response.Item1[1] != "0")
            {
                ThrowFailedResponse("Failed to clear mobile buffers.", response);
            }
        }

        public void Configure()
        {
            EchoEnable = true;
            ResponseSuppression = false;
            Verbosity = ISU.Verbosities.Verbose;
            RadioEnable = true;
        }

        public void PrintDeviceInfo()
        {
            Console.WriteLine("ISN                      : " + ISN);
            Console.WriteLine("Software revision        : " + SoftwareRevision);
            Console.WriteLine("Product family           : " + ProductFamily);
            Console.WriteLine("Factory identity         : " + FactoryIdentity);
            Console.WriteLine("Hardware specification   : " + HardwareSpecification);
            Console.WriteLine();
        }

        public void PrintSignalQuality()
        {
            Console.WriteLine("Signal quality: " + SignalQuality);
            Console.WriteLine();
        }

        public bool HasSignal
        {
            get { return SignalQuality > 0; }
        }

        public class ISUResult
        {
            public string Message { get; set; }
            public bool ReceiveSuccessful { get; set; }
            public bool SendSuccessful { get; set; }
            public int MessagesRemaining { get; set; }
        }

        public ISUResult SendAndReceiveMessage(string messageToSend = null)
        {
            ClearMobileBuffers();

            if (!HasSignal)
            {
                throw new Exception("No signal.");
            }

            if (messageToSend != null)
            {
                WriteMessageData(Encoding.ASCII.GetBytes(messageToSend));
            }
            var status = InitiateSBDSession();
            bool sendOK = true;
            if (messageToSend != null)
            {
                sendOK = status.Item1 == 1;
            }
            else
            {
                sendOK = status.Item1 == 0;
            }

            bool receiveOK = status.Item3 != 2;
            string messageReceived = null;
            if (status.Item3 == 1)
            {
                byte[] data = ReadMessageData();
                messageReceived = Encoding.ASCII.GetString(data);
            }
            return new ISUResult
            {
                Message = messageReceived,
                ReceiveSuccessful = receiveOK,
                SendSuccessful = sendOK,
                MessagesRemaining = status.Item6
            };
        }

        public void Dispose()
        {
            port.Close();
        }
    }
}