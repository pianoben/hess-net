using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HessNet.Runtime.Serialization.Hessian
{
    public class StringChunk : OutputChunk
    {
        private readonly int charCount;

        public int CharCount
        {
            get { return charCount; }
        }

        public StringChunk(bool isFinal, char[] data)
            : base(Encoding.UTF8.GetByteCount(data), isFinal, Encoding.UTF8.GetBytes(data))
        {
            this.charCount = data.Length;
        }
    }
}
