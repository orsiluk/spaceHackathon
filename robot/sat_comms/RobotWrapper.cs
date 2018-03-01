using System;

namespace sat_comms
{
    interface IRobot : IDisposable
    {
        string Name { get; }
        void DriveLeft(int degrees);
        void DriveRight(int degrees);
        void DriveForward(int distance);
        void DriveBackward(int distance);
    }

    class RobotMock : IRobot
    {
        public void Dispose()
        {
        }

        public void DriveLeft(int degrees)
        {
            Console.WriteLine($"Pretending to turn left {degrees}deg");
        }

        public void DriveRight(int degrees)
        {
            Console.WriteLine($"Pretending to turn right {degrees}deg");
        }

        public void DriveForward(int distance)
        {
            Console.WriteLine($"Pretending to drive forward {distance}cm");
        }

        public void DriveBackward(int distance)
        {
            Console.WriteLine($"Pretending to drive backward {distance}cm");
        }

        public string Name => "FakeRobot";
    }

    class RobotWrapper : IRobot
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

        public void DriveLeft(int degrees)
        {
            RobotLib.Robot.DriveLeft(degrees);
        }

        public void DriveRight(int degrees)
        {
            RobotLib.Robot.DriveRight(degrees);
        }

        public void DriveForward(int distance)
        {
            RobotLib.Robot.DriveForward(distance);
        }

        public void DriveBackward(int distance)
        {
            RobotLib.Robot.DriveBackward(distance);
        }

        public string Name => RobotLib.Robot.Name;
    }
}