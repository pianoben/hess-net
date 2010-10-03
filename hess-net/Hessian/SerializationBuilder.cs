using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HessNet.Hessian
{
    public class SerializationBuilder : IDisposable
    {
        private bool disposed;
        private readonly MemoryStream ms;
        private readonly Dictionary<object, uint> objectMap;
        private readonly Dictionary<Type, uint> classMap;

        public SerializationBuilder()
        {
            ms = new MemoryStream();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (ms != null) ms.Dispose();
                }

                disposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}
