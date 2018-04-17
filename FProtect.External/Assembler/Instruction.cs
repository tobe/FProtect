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
        EAX,
        EBX,
        ECX,
        EDX,
        ESI,
        EDI
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

        }
    }
}
