using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FProtect.External.Assembler
{
    public class Instruction32 : Instruction
    {
        public Instruction32(Mnemonics Mnemonic, Registers? Register, byte? Value = null)
            : base(Mnemonic, Register, Value)
        {
            this.GenerateByteCode();
        }

        /// <summary>
        /// Generates bytecode based on the supplied information
        /// </summary>
        private void GenerateByteCode()
        {
            // Find out which mnemonic is being assembled and call the correct method
            switch (this._mnemonic)
            {
                case Mnemonics.ADD:
                    this.Arithmetics(true);
                    break;
                case Mnemonics.SUB:
                    this.Arithmetics(false);
                    break;
            }
        }

        private void Arithmetics(bool IsAddition)
        {
            // opcode register byte 00 00 00
            this._byteCode = new byte[] { 0x81, 0x00, 0x00, 0x00, 0x00, 0x00 };

            // Fix the register and the actual value
            this._byteCode[1] = this.GetRegisterOpcode(Mnemonics.ADD);
            this._byteCode[2] = (byte)this._value;

            // Check if the register is EAX, if it is apply a random special transformation
            if (Instruction._random.NextDouble() < 0.5) return;
            if (this._register == Registers.EAX)
                this._byteCode = new byte[] {
                    IsAddition ? (byte)0x05 : (byte)0x2d,
                    this._byteCode[2],
                    0x00, 0x00, 0x00
                };
        }
    }
}
