using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NKH.MindSqualls;
using RobotLib;

namespace RoverServer
{
    public static class Robot
    {
        public static RobotLib.Robot.CommsMode Mode
        {
            get
            {
                return RobotLib.Robot.Mode;
            }
        }

        public static void Init()
        {
            RobotLib.Robot.Init();
        }

        public static void Cleanup()
        {
            RobotLib.Robot.Cleanup();
        }

        public static void HandleCommand(RoverServer.Controllers.Command command)
        {
            // TODO
            int data = int.Parse(command.Data);
            switch (command.CommandType)
            {
                case Controllers.CommandType.Forward:
                    RobotLib.Robot.DriveForward(data);
                    break;
                case Controllers.CommandType.Backward:
                    RobotLib.Robot.DriveBackward(data);
                    break;
                case Controllers.CommandType.Left:
                    RobotLib.Robot.DriveLeft(data);
                    break;
                case Controllers.CommandType.Right:
                    RobotLib.Robot.DriveRight(data);
                    break;
                default:
                    throw new ArgumentException("Unrecognised command type.");
            }
        }
    }
}