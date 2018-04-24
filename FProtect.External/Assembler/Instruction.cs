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
        RAX = 0,

        ECX = 1,
        RCX = 1,

        EDX = 2,
        RDX = 2
    }

    public class Instruction : IInstruction
    {
        private byte[] _byteCode;
        public byte[] ByteCode
        {
            get { return _byteCode; }
            set { _byteCode = value; }
        }

        private Mnemonics _mnemonic;
        private Registers? _register;
        private byte? _value;
        private bool _is64Bit;

        private static Random _random = new Random();

        public Instruction(Mnemonics Mnemonic, Registers? Register, byte? Value = null, bool Is64Bit = false)
        {
            this._mnemonic = Mnemonic;
            // If a register hasn't been supplied, this means it's a special instruction.
            this._register = Register;
            // If a value hasn't been supplied, this means it's a unary mnemonic.
            this._value = Value;
            this._is64Bit = Is64Bit;

            this.GenerateByteCode();
        }

        /// <summary>
        /// Gets the correct register opcode depending on the operation
        /// </summary>
        /// <param name="Mnemonic">The mnemonic of the operation</param>
        /// <returns>The correct register opcode for a mnemonic</returns>
        private byte GetRegisterOpcode(Mnemonics Mnemonic)
        {
            if (Mnemonic == Mnemonics.ADD)
                return (byte)(0xc0 + (int)this._register);
            if (Mnemonic == Mnemonics.SUB)
                return (byte)(0xe8 + (int)this._register);
            if (Mnemonic == Mnemonics.XOR)
                return (byte)(0xf0 + (int)this._register);

            return 0;
        }

        /// <summary>
        /// Prepends a byte to the bytecode array
        /// </summary>
        /// <param name="Value">Byte to prepend</param>
        private void PrependBytecode(byte Value)
        {
            byte[] newBytecode = new byte[this._byteCode.Length + 1];
            newBytecode[0] = Value;
            Array.Copy(this._byteCode, 0, newBytecode, 1, this._byteCode.Length);
            this._byteCode = newBytecode;
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
                case Mnemonics.XOR:
                    this.Xor();
                    break;
            }
        }

        private void Arithmetics(bool IsAddition)
        {
            // opcode register byte 00 00 00
            this._byteCode = new byte[] { 0x81, 0xeb, 0x00, 0x00, 0x00, 0x00 };

            // Fix the register and the actual value
            if (IsAddition)
                this._byteCode[1] = this.GetRegisterOpcode(Mnemonics.ADD);
            else
                this._byteCode[1] = this.GetRegisterOpcode(Mnemonics.SUB);
            this._byteCode[2] = (byte)this._value;

            // Check if the register is RAX/EAX, if it is apply a random special transformation
            bool transformEax = Instruction._random.NextDouble() >= 0.5 ? true : false;
            if ((this._register == Registers.EAX || this._register == Registers.RAX) && transformEax)
            {
                this._byteCode = new byte[] {
                        IsAddition ? (byte)0x05 : (byte)0x2d,
                        this._byteCode[2],
                        0x00, 0x00, 0x00
                    };
            }

            // If we are dealing with a 64 bit wide register, append 0x48 in front
            if (this._is64Bit)
                this.PrependBytecode(0x48);
        }

        private void Xor()
        {
            // opcode register byte 00 00 00
            this._byteCode = new byte[] { 0x81, 0x00, 0x00, 0x00, 0x00, 0x00 };

            // Fix the first opcode to get the right register
            this._byteCode[1] = this.GetRegisterOpcode(Mnemonics.XOR);

            // Add the byte
            this._byteCode[2] = (byte)this._value;

            // Check if the register is RAX/EAX, if it is apply a random special transformation
            bool transformEax = Instruction._random.NextDouble() >= 0.5 ? true : false;
            if ((this._register == Registers.EAX || this._register == Registers.RAX) && transformEax)
            {
                this._byteCode = new byte[] {
                        0x35,
                        this._byteCode[2],
                        0x00, 0x00, 0x00
                    };
            }

            // If we are dealing with a 64 bit wide register, append 0x48 in front
            if (this._is64Bit)
                this.PrependBytecode(0x48);
        }
    }
}