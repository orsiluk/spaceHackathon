using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sat_comms
{
    class Program
    {
        static void Main(string[] args)
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

                Console.WriteLine("Attempting to read ISN...");
                string ISN = isu.ISN;
                Console.WriteLine("Done. ISN is: " + ISN);
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (isu != null)
                {
                    isu.Close();
                    isu = null;
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();

                Console.WriteLine("Application will now exit.");
            }
        }
    }
}
