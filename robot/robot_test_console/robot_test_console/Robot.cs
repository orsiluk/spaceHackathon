using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NKH.MindSqualls;

namespace RobotLib
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

        private static void ResetPosition()
        {
            motorPair.Run(100, 10, 0);
            System.Threading.Thread.Sleep(200);
            motorPair.Idle();
            System.Threading.Thread.Sleep(200);
        }
        
        public static void DriveForward(int distance)
        {
            // Approx. 10cm per 360 degrees
            motorPair.Run(100, (ushort)(36 * distance), 0);
            // Approx. 100ms per cm
            System.Threading.Thread.Sleep((distance * 100) + 500);

            ResetPosition();
        }

        public static void DriveBackward(int distance)
        {
            // Approx. 10cm per 360 degrees
            motorPair.Run(-100, (ushort)(36 * distance), 0);
            // Approx. 100ms per cm
            System.Threading.Thread.Sleep(distance * 100 + 500);

            ResetPosition();
        }

        public static void DriveLeft(int degrees)
        {
            motorPair.Run(100, (ushort)(5 * degrees), 100);
            System.Threading.Thread.Sleep(degrees * 10 + 500);

            ResetPosition();
        }

        public static void DriveRight(int degrees)
        {
            motorPair.Run(100, (ushort)(5 * degrees), -100);
            System.Threading.Thread.Sleep(degrees * 10 + 500);

            ResetPosition();
        }
    }
}