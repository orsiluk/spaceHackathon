using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NKH.MindSqualls;

namespace robot_test_console
{
    class Program
    {
        static void Main(string[] args)
        {
            RobotLib.Robot.Init();

            if (RobotLib.Robot.Mode != RobotLib.Robot.CommsMode.NXT)
            {
                Console.WriteLine("Robot not connected. Is it plugged in?");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            
            // Query the device info, and write out the NXT's name.
            Console.WriteLine("NXT name: {0}", RobotLib.Robot.Name);

            Console.WriteLine("Running...");

            do
            {
                //Console.WriteLine("Drive 1cm");
                //RobotLib.Robot.DriveForward(1);

                //Console.WriteLine("Drive 2cm");
                //RobotLib.Robot.DriveForward(2);

                //Console.WriteLine("Drive 4cm");
                //RobotLib.Robot.DriveForward(4);

                //Console.WriteLine("Drive 8cm");
                //RobotLib.Robot.DriveForward(8);

                //Console.WriteLine("Drive 16cm");
                //RobotLib.Robot.DriveForward(16);

                //Console.WriteLine("Drive 15 degrees");
                //RobotLib.Robot.DriveLeft(15);

                Console.WriteLine("Drive 30 degrees");
                RobotLib.Robot.DriveLeft(30);

                Console.WriteLine("Drive 45 degrees");
                RobotLib.Robot.DriveRight(45);

                Console.WriteLine("Drive 60 degrees");
                RobotLib.Robot.DriveLeft(60);

                Console.WriteLine("Drive 90 degrees");
                RobotLib.Robot.DriveLeft(90);

                Console.WriteLine("Drive 120 degrees");
                RobotLib.Robot.DriveLeft(120);

                Console.WriteLine("Drive 180 degrees");
                RobotLib.Robot.DriveRight(180);

                Console.WriteLine("Drive 360 degrees");
                RobotLib.Robot.DriveLeft(360);

                Console.WriteLine("Press Q to quit or any other key to repeat the test.");
            }
            while (Console.ReadKey().Key != ConsoleKey.Q);

            RobotLib.Robot.Cleanup();
        }
    }
}
