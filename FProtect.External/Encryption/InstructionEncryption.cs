using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FProtect.External.Assembler;

namespace FProtect.External.Encryption
{
    public class InstructionEncryption
    {
        private byte[] _data;
        private int _rounds;
        private bool _is64bit;

        private Random _random = new Random();

        private List<byte> _decryptionStub = new List<byte>();
        /*public byte[] DecryptionStub
        {
            get
            {
                // Convert list to bytearray and return
                
            }
        }*/

        public InstructionEncryption(byte[] Data, int Rounds, bool Is64Bit)
        {
            this._data = Data;
            this._rounds = Rounds;
            this._is64bit = Is64Bit;
        }

        public void EncryptRoutine(Dictionary<string, uint> Routine)
        {
            byte[] entropy = new byte[32];
            byte[] randomOrder = new byte[32];
            for(int i = 0; i < 32; i++)
            {
                entropy[i]      = (byte)this._random.Next(0, 255);
                randomOrder[i]  = (byte)this._random.Next(0, 4);
            }
            Registers randomRegister = Registers.EAX; // TODO: fix this

            RoutineInfo routineInfo = new RoutineInfo()
            {
                Entropy = entropy,
                RandomOrder = randomOrder,
                Register = randomRegister
            };

            for(uint i = Routine["start"]; i < Routine["end"]; i++)
            {
                routineInfo.Byte = this._data[i];
                byte encryptedByte = this.EncryptionStub(routineInfo);
                Console.WriteLine("oldByte: {0:X2} newByte:; {1:X2}\n", routineInfo.Byte, encryptedByte);
            }

            foreach(var _byte in this._decryptionStub)
            {
                Console.Write("{0:X2} ", _byte);
            }
        }

        // deterministic function za svaki bajt idu iste instrukcije!
        private byte EncryptionStub(RoutineInfo RoutineInfo)
        {
            byte newByte = RoutineInfo.Byte;
            Instruction instruction = null;
            List<byte> decryptionStub = new List<byte>();

            for(int i = 0; i < this._rounds; i++)
            {
                switch(RoutineInfo.RandomOrder[i])
                {
                    case 0: // ADD
                        newByte = (byte)(newByte + RoutineInfo.Entropy[i]);
                        instruction = new Instruction(
                            Mnemonics.SUB,
                            RoutineInfo.Register,
                            RoutineInfo.Entropy[i],
                            this._is64bit
                        );

                        Console.WriteLine("SUB {0:X2}", RoutineInfo.Entropy[i]);

                        break;
                    case 1: // SUB
                        newByte = (byte)(newByte - RoutineInfo.Entropy[i]);
                        instruction = new Instruction(
                            Mnemonics.ADD,
                            RoutineInfo.Register,
                            RoutineInfo.Entropy[i],
                            this._is64bit
                        );

                        Console.WriteLine("ADD {0:X2}", RoutineInfo.Entropy[i]);

                        break;
                    case 2: // XOR
                        newByte = (byte)(newByte ^ RoutineInfo.Entropy[i]);
                        instruction = new Instruction(
                            Mnemonics.XOR,
                            RoutineInfo.Register,
                            RoutineInfo.Entropy[i],
                            this._is64bit
                        );

                        Console.WriteLine("e/d: XOR {0:X2}", RoutineInfo.Entropy[i]);

                        break;
                    case 3: // NOT
                        //newByte = (byte)(~RoutineInfo.Entropy[i]);
                        newByte = unchecked((byte)(~newByte));
                        instruction = new Instruction(
                            Mnemonics.NOT,
                            RoutineInfo.Register,
                            null,
                            this._is64bit
                        );

                        Console.WriteLine("e/d: NOT {0:X2}", RoutineInfo.Entropy[i]);

                        break;
                }

                //decryptionStub.AddRange(instruction.ByteCode);
                decryptionStub.InsertRange(0, instruction.ByteCode);
            }

            this._decryptionStub = decryptionStub;

            Console.WriteLine("\n");

            return newByte;
        }

        /// <summary>
        /// https://stackoverflow.com/a/3132139
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns>A random enum</returns>
        static T RandomEnumValue<T>()
        {
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(new Random().Next(v.Length));
        }
    }
}
