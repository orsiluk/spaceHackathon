using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sat_comms
{
    class Program
    {
        public static void Main(string[] args)
        {
            ISU isu = null;

            try
            {
                if (args.Length < 1)
                {
                    throw new Exception("Incorrect number of command line args. Args: COMPort");
                }

                isu = new ISU(args[0]);
                isu.Open();

                isu.Configure();
                isu.PrintDeviceInfo();
                isu.PrintSignalQuality();
                
                bool messagesRemaining = true;
                while(messagesRemaining)
                {
                    var result = isu.SendAndReceiveMessage();
                    if (result.Message != null)
                    {
                        Console.WriteLine("Received message: " + result.Message);
                    }
                    messagesRemaining = result.MessagesRemaining > 0;
                }

                // Use to send a message: var result = isu.SendAndReceiveMessage("My message");
                //  But remember to check if a message was read at the same time afterwards!
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                isu?.Close();

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();

                Console.WriteLine("Application will now exit.");
            }
        }
    }
}
