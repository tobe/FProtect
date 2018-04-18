using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FProtect.External.Assembler
{
    public enum Mnemonics
    {
        ADD,
        SUB,
        XOR,
        NOT,
    }

    public enum Registers
    {
        EAX = 0xc0,
        EBX = 0xc3,
        ECX = 0xc1,
        EDX = 0xc2,
        ESI = 0xc6,
        EDI = 0xc7
    }

    public class Instruction
    {
        private byte[] _byteCode;
        public byte[] ByteCode
        {
            get { return _byteCode; }
            set { _byteCode = value; }
        }

        private Mnemonics _mnemonic;
        private Registers _register;
        private byte? _value;

        private static Random _random = new Random();

        public Instruction(Mnemonics Mnemonic, Registers Register, byte? Value = null)
        {
            this._mnemonic = Mnemonic;
            this._register = Register;
            // If a value hasn't been supplied, this means it's a unary mnemonic.
            this._value = Value;

            // Generate bytecode automatically for us
            this.GenerateByteCode();
        }

        /// <summary>
        /// Generates bytecode based on the supplied information
        /// </summary>
        private void GenerateByteCode()
        {
            // Find out which mnemonic is being assembled and call the correct method
            switch(this._mnemonic)
            {
                case Mnemonics.ADD:
                    this.Add();        
                break;
            }
        }

        private void Add()
        {
            // opcode register byte 00 00 00
            this._byteCode = new byte[] { 0x81, 0x00, 0x00, 0x00, 0x00, 0x00 };

            // Fix the register and the actual value
            this._byteCode[1] = (byte)this._register;
            this._byteCode[2] = (byte)this._value;

            // Check if the register is EAX, if it is apply a random special transformation
            if (this._register == Registers.EAX && Instruction._random.NextDouble() > 0.5)
                this._byteCode = new byte[] { 0x05, this._byteCode[2], 0x00, 0x00, 0x00 };
        }

        private void Sub()
        {
            // opcode register byte 00 00 00
            this._byteCode = new byte[] { };
        }
    }
}
