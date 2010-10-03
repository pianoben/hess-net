using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HessNet.Runtime.Serialization.Hessian
{
    public class HessianStream : IDisposable
    {
        private bool disposed;
        private Stream source;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public HessianToken NextToken()
        {
            var b = source.ReadByte();

            if (b == -1)
                goto EOF;


        EOF:
            throw new EndOfStreamException();
        }

        protected virtual void Dispose(bool disposing)
        {

        }
    }
}
