using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace sat_comms
{
    class Application
    {
        readonly ISU _isu;
        private bool _running = true;

        public Application(ISU isu)
        {
            _isu = isu;
        }

        public void CheckMessages()
        {
            var messagesRemaining = true;
            while (messagesRemaining)
            {
                var result = _isu.SendAndReceiveMessage();
                if (result.Message != null)
                {
                    Console.WriteLine("Received message: " + result.Message);
                }
                messagesRemaining = result.MessagesRemaining > 0;
            }
        }

        public void Run()
        {
            var thread = new Thread(() =>
            {
                _isu.Configure();
                _isu.PrintDeviceInfo();
                _isu.PrintSignalQuality();

                Console.WriteLine("Starting Main Loop:");

                while (_running)
                {
                    Thread.Sleep(1000);
                    CheckMessages();
                }
            });
        }

        public void Stop()
        {
            _running = false;
            _isu?.Close();
        }
    }

    class Program
    {
        public static ISU GetIsu(string comPort)
        {
            var isu = new ISU(comPort);
            try
            {
                isu.Open();
            }
            catch (Exception)
            {
                isu.Close();
            }
            return isu;
        }

        public static void Main(string[] args)
        {
            var exitEvent = new ManualResetEvent(false);

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                exitEvent.Set();
            };

            try
            {
                if (args.Length < 1)
                {
                    throw new Exception("Incorrect number of command line args. Args: COMPort");
                }

                Console.WriteLine("Initialising ISU:");
                using (var isu = GetIsu(args[0]))
                {
                    var app = new Application(isu);
                    app.Run();

                    exitEvent.WaitOne();
                    app.Stop();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();

                Console.WriteLine("Application will now exit.");
            }
        }
    }
}