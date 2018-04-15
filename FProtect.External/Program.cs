#define TESTING
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace FProtect.External
{
    class Program
    {
        static void Main(string[] args)
        {
            // Grab the file
#if !TESTING
            var fileName = args[1];
            if(string.IsNullOrEmpty(fileName))
            {
                Console.WriteLine("Cannot continue without a file name.");
                return;
            }
#endif
#if TESTING
            var fileName = "D:\\FProtect\\Release\\FProtect.Test.exe";
#endif

            // Check if the file exists
            if (!File.Exists(fileName))
            {
                Console.WriteLine("Cannot find the file: " + fileName);
                return;
            }

            // Try to read all the bytes from the file
            byte[] data = null;
            try
            {
                data = File.ReadAllBytes(fileName);
            }catch(IOException e)
            {
                Console.WriteLine("An I/O Error occured while trying to read the file: " + e.Message);
                return;
            }catch(SecurityException e)
            {
                Console.WriteLine("The caller does not have the required permission to read the file: " + e.Message);
                return;
            }

            var testing = Parser.Initialize(data);
        }
    }
}
