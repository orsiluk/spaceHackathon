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
            // Create a NXT brick,
            // and use USB to communicate with it.
            NxtBrick brick = new NxtBrick(NxtCommLinkType.USB, 0);

            // Attach motors to port B and C on the NXT.
            brick.MotorB = new NxtMotor();
            brick.MotorC = new NxtMotor();
            
            // Syncronize the two motors.
            NxtMotorSync motorPair = new NxtMotorSync(brick.MotorB, brick.MotorC);

            brick.Connect();

            // Query the device info, and write out the NXT's name.
            Console.WriteLine("NXT name: {0}", brick.Name);

            Console.WriteLine("Running motors B and C...");

            //brick.MotorB.Run(50, 360);

            // Run them at 75% power, for a 3600 degree run.
            while (true)
            {
                motorPair.Run(100, 3600, -100);
                Console.WriteLine("Running...");

                // Wait 8 seconds before putting the motors into idle-mode.
                System.Threading.Thread.Sleep(7 * 1000);
                motorPair.Run(100, 10, 0);
                System.Threading.Thread.Sleep(200);
                motorPair.Idle();
                System.Threading.Thread.Sleep(200);
            }

            brick.MotorB.Idle();
            brick.MotorC.Idle();
            //motorPair.Idle();

            Console.WriteLine("Done.");

            // Disconnect from the NXT.
            brick.Disconnect();

            Console.ReadLine();
        }
    }
}
