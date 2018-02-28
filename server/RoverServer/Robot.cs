using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NKH.MindSqualls;

namespace RoverServer
{
    public static class Robot
    {
        public enum CommsMode
        {
            SAT,
            NXT
        }

        private static NxtBrick brick;
        private static NxtMotorSync motorPair;

        public static CommsMode Mode
        {
            get
            {
                return brick.IsConnected ? CommsMode.NXT : CommsMode.SAT;
            }
        }

        public static string Name
        {
            get
            {
                return brick.Name;
            }
        }

        public static void Init()
        {
            brick = new NxtBrick(NxtCommLinkType.USB, 0);
            
            // Attach motors to port B and C on the NXT.
            brick.MotorB = new NxtMotor();
            brick.MotorC = new NxtMotor();

            // Syncronize the two motors.
            motorPair = new NxtMotorSync(brick.MotorB, brick.MotorC);

            brick.Connect();
        }

        public static void Cleanup()
        {
            brick.Disconnect();
        }

        public static void HandleCommand(RoverServer.Controllers.Command command)
        {
            // TODO
        }

        public static void DriveForward(int distance)
        {
            // TODO: Distance per degree of turn of wheels -> degrees requested -> some actual number of degrees to turn
            motorPair.Run(100, 360, 0);
        }

        public static void DriveBackward(int distance)
        {
            // TODO: Distance per degree of turn of wheels -> degrees requested -> some actual number of degrees to turn
            motorPair.Run(-100, 360, 0);
        }

        public static void DriveLeft(int degrees)
        {
            // TODO: Distance per degree of turn of wheels -> degrees requested -> some actual number of degrees to turn
            motorPair.Run(-100, 360, 100);
        }

        public static void DriveRight(int degrees)
        {
            // TODO: Distance per degree of turn of wheels -> degrees requested -> some actual number of degrees to turn
            motorPair.Run(-100, 360, -100);
        }
    }
}