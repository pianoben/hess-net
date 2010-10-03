using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HessNet.Runtime.Serialization.Hessian
{
    public class OutputChunk
    {
        public const int MaxChunkSize = ushort.MaxValue;

        protected readonly byte[] data;
        protected readonly int length;
        protected readonly bool isFinal;

        public byte[] Data
        {
            get { return data; }
        }

        public int Length
        {
            get { return length; }
        }

        public bool IsFinal
        {
            get { return isFinal; }
        }

        public OutputChunk(int length, bool isFinal, byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (length < 0)
                throw new ArgumentOutOfRangeException("length");

            this.length = length;
            this.isFinal = isFinal;
            this.data = data;
        }
    }
}
