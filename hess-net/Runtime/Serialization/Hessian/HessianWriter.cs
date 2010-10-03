using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace HessNet.Runtime.Serialization.Hessian
{
    public class HessianWriter : HessianSerializerBase, IDisposable
    {
        private bool disposed;
        private Stack<HessianType> elements;
        private bool closeOutput;
        private Stream output;

        public HessianWriter(Stream output, bool closeOutput)
        {
            this.elements = new Stack<HessianType>();
            SetOutput(output, closeOutput);
        }

        public void SetOutput(Stream output, bool ownsOutput)
        {
            if (output == null)
                throw new ArgumentNullException("output");

            this.output = output;
            this.closeOutput = ownsOutput;
        }

        public void WriteObject(object graph)
        {
            if (graph == null)
            {

            }
        }

        private void WriteChar(char c)
        {
            output.WriteByte((byte)c);
        }

        private void WriteLength(ushort length)
        {
            output.WriteByte((byte)(length >> 8));
            output.WriteByte((byte)(length & ~(byte.MaxValue)));
        }

        private void WriteString(string s)
        {
            var chunks = ChunkString(s);
        }

        private void WriteChunkedData(IEnumerable<OutputChunk> chunks)
        {
            var firstChunk = true;

            foreach (var c in chunks)
            {
                if (c.IsFinal)
                {
                    // Is this the first (and therefore only) chunk?  If so,
                    // can we write this string in compact representation?
                    if (firstChunk && c.Length < 32)
                    {
                        // In compact string representation, a string 31 chars
                        // or shorter is encoded as the number of UTF-8 bytes
                        output.WriteByte((byte)c.Length);

                        if (c.Length != 0)
                        {
                            output.Write(c.Data, 0, c.Length);
                        }
                    }

                    WriteChar('S');
                }
                else
                {
                    WriteChar('s');
                }

                WriteLength(c.Length);

                output.Write(c.Data, 0, c.Length);
                firstChunk = false;
            }
        }

        #region IDisposable Implementation

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
                    if (closeOutput && output != null)
                    {
                        output.Dispose();
                        output = null;
                    }
                }

                GC.SuppressFinalize(this);
                this.disposed = true;
            }
        }

        #endregion IDisposable Implementation
    }
}
