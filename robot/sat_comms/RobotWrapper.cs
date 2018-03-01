using System;

namespace sat_comms
{
    class RobotWrapper : IDisposable
    {
        public RobotWrapper()
        {
            RobotLib.Robot.Init();

            if (RobotLib.Robot.Mode != RobotLib.Robot.CommsMode.NXT)
            {
                Console.WriteLine("Robot not connected. Is it plugged in?");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }

            Console.WriteLine("NXT name: {0}", RobotLib.Robot.Name);
        }

        public void Dispose()
        {
            RobotLib.Robot.Cleanup();
        }
    }
}
