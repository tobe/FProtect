using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FProtect.External
{
    static public class Parser
    {
        public static List<Dictionary<string, UInt32>> Initialize(byte[] Data)
        {
            var results = new List<Dictionary<string, UInt32>>();
            bool foundStart = false;
            UInt32 startAddress = 0;

            for(UInt32 i = 0; i < Data.Length; i++)
            {
                if(Parser.HasStart(Data, i))
                {
                    foundStart = true;
                    startAddress = i;
                }

                if(Parser.HasEnd(Data, i) && foundStart)
                {
                    // Found the end of the function, create a new dictionary for it and add it
                    results.Add(new Dictionary<string, uint>
                    {
                        {"start", startAddress},
                        {"end",   i},
                        {"size",  i - startAddress },
                        {"hash",  0} // TODO: Add this
                    });

                    foundStart = false;
                    startAddress = 0;
                }
            }

            return results;
        }

        private static bool HasStart(byte[] ByteArray, UInt32 Offset)
        {
            return ByteArray[Offset] == 0x12 &&
                   ByteArray[Offset + 1] == 0x34 &&
                   ByteArray[Offset + 2] == 0x56;
        }

        private static bool HasEnd(byte[] ByteArray, UInt32 Offset)
        {
            return ByteArray[Offset] == 0x56 &&
                   ByteArray[Offset + 1] == 0x34 &&
                   ByteArray[Offset + 2] == 0x12;
        }
    }
}
