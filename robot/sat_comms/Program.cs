﻿using System;
using System.Threading;

namespace sat_comms
{
    class Application
    {
        readonly IISU _isu;
        private readonly RobotWrapper _robot;
        private bool _running = true;

        public Application(IISU isu, RobotWrapper robot)
        {
            _isu = isu;
        }

        private void HandleMessage(string message)
        {
            if (message[0] != 'B')
            {
                Console.WriteLine("Ignored non-B message");
                return;
            }

            var segments = message.Split(':');
            if (segments.Length != 3)
            {
                Console.WriteLine($"Invalid message {message}");
                return;
            }

            if (!int.TryParse(segments[2], out int payload))
            {
                Console.WriteLine($"Invalid payload in {message}");
                return;
            }

            switch (segments[1])
            {
                case "FORWARD":
                    Console.WriteLine($"Driving forward {payload}cm");
                    RobotLib.Robot.DriveForward(payload);
                    break;
                case "BACKWARD":
                    Console.WriteLine($"Driving backward {payload}cm");
                    RobotLib.Robot.DriveBackward(payload);
                    break;
                case "LEFT":
                    Console.WriteLine($"Turning left {payload}deg");
                    RobotLib.Robot.DriveLeft(payload);
                    break;
                case "RIGHT":
                    Console.WriteLine($"Turning right {payload}deg");
                    RobotLib.Robot.DriveRight(payload);
                    break;
            }
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
                    HandleMessage(result.Message);
                }
                messagesRemaining = result.MessagesRemaining > 0;
            }
        }

        public void Run()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                _isu.Configure();
                _isu.PrintDeviceInfo();
                _isu.PrintSignalQuality();

                Console.WriteLine("Starting Main Loop:");

                while (_running)
                {
                    Thread.Sleep(10000);
                    CheckMessages();
                }
            }).Start();
        }

        public void Stop()
        {
            _running = false;
        }
    }

    class Program
    {
        public static IISU GetIsu(string comPort)
        {
            if (comPort == "mock")
            {
                return new MockISU();
            }
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

                Console.WriteLine("Initialising ISU");
                using (var isu = GetIsu(args[0]))
                using (var robot = new RobotWrapper())
                {
                    var app = new Application(isu, robot);
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