using FProtect.External.Assembler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FProtect.External.Encryption
{
    public class RoutineInfo
    {
        /// <summary>
        /// Entropy used for XOR / ADD / SUB functions
        /// </summary>
        private byte[] _entropy;
        public byte[] Entropy
        {
            get { return this._entropy; }
            set { this._entropy = value; }
        }

        /// <summary>
        /// The order of instructions which manipulate each byte
        /// </summary>
        private byte[] _randomOrder;
        public byte[] RandomOrder
        {
            get { return this._randomOrder; }
            set { this._randomOrder = value; }
        }

        private Registers _register;
        public Registers Register
        {
            get { return this._register; }
            set { this._register = value; }
        }

        private byte _byte;
        public byte Byte
        {
            get { return this._byte; }
            set { this._byte = value; }
        }

    }
}
