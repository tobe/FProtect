using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FProtect.External.Assembler
{
    public class InstructionAssembler
    {
        private List<Instruction> _instructionList;
        public List<Instruction> InstructionList
        {
            get { return this._instructionList; }
        }

        public InstructionAssembler(List<Instruction> InstructionList)
        {
            this._instructionList = InstructionList;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            
            foreach(var instruction in this._instructionList)
            {
                foreach (var _byte in instruction.ByteCode)
                {
                    stringBuilder.AppendFormat("{0:X2} ", _byte);
                }
                stringBuilder.Append("\n");
            }

            return stringBuilder.ToString();
            return base.ToString();
        }
    }
}
