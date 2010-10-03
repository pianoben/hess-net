using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace HessNet.Runtime.Serialization.Hessian
{
    public abstract class HessianToken
    {
        public abstract IEnumerable<byte> GetBytes();
        public abstract void Read(Stream source);

        protected void CheckEndOfStream(int value)
        {
            if (value == -1)
            {
                throw new SerializationException("Unexpected end of stream");
            }
        }
    }
}
