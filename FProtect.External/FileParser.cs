using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FProtect.External
{
    static public class FileParser
    {
        /// <summary>
        /// "Magic bytes" which mark the start and the end of a routine
        /// </summary>
        private const byte _magicByte1 = 0xF4;
        private const byte _magicByte2 = 0x0F;
        private const byte _magicByte3 = 0xAA;

        /// <summary>
        /// Parses the executable file
        /// </summary>
        /// <param name="Data">Bytes of the file</param>
        /// <returns>A list of found routines</returns>
        public static List<Dictionary<string, UInt32>> Initialize(byte[] Data)
        {
            var results = new List<Dictionary<string, UInt32>>();
            bool foundStart = false;
            UInt32 startAddress = 0;

            for(UInt32 i = 0; i < Data.Length; i++)
            {
                if(FileParser.HasStart(Data, i))
                {
                    foundStart = true;
                    startAddress = i;
                }

                if(FileParser.HasEnd(Data, i) && foundStart)
                {
                    // Found the end of the routine, create a new dictionary for it and add it
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
            return ByteArray[Offset] == _magicByte1 &&
                   ByteArray[Offset + 1] == _magicByte2 &&
                   ByteArray[Offset + 2] == _magicByte3;
        }

        private static bool HasEnd(byte[] ByteArray, UInt32 Offset)
        {
            return ByteArray[Offset] == _magicByte2 &&
                   ByteArray[Offset + 1] == _magicByte3 &&
                   ByteArray[Offset + 2] == _magicByte1;
        }
    }
}
