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
        EAX = 0,
        EBX = 3,
        ECX = 1,
        EDX = 2,
        ESI = 6,
        EDI = 7
    }

    public abstract class Instruction : IInstruction
    {
        protected byte[] _byteCode;
        public byte[] ByteCode
        {
            get { return _byteCode; }
            set { _byteCode = value; }
        }

        protected Mnemonics _mnemonic;
        protected Registers? _register;
        protected byte? _value;

        protected static Random _random = new Random();

        public Instruction(Mnemonics Mnemonic, Registers? Register, byte? Value = null)
        {
            this._mnemonic = Mnemonic;
            // If a register hasn't been supplied, this means it's a special instruction.
            this._register = Register;
            // If a value hasn't been supplied, this means it's a unary mnemonic.
            this._value = Value;
        }

        /// <summary>
        /// Gets the correct register opcode depending on the operation
        /// </summary>
        /// <param name="Mnemonic">The mnemonic of the operation</param>
        /// <returns>The correct register opcode for a mnemonic</returns>
        protected byte GetRegisterOpcode(Mnemonics Mnemonic)
        {
            if (Mnemonic == Mnemonics.ADD)
                return (byte)(0xc0 + (int)this._register);
            if (Mnemonic == Mnemonics.SUB)
                return (byte)(0xe8 + (int)this._register);

            return 0;
        }
    }
}