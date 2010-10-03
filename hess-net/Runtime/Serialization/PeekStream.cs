using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HessNet.Runtime.Serialization
{
    /// <summary>
    /// A read-only stream that provides a one-byte lookahead feature.
    /// </summary>
    public class PeekStream : Stream
    {
        private Stream innerStream;
        private byte? peeked = null;
        private bool disposed = false;

        public override bool CanRead
        {
            get { return innerStream.CanRead; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get { return innerStream.Length + (peeked.HasValue ? 1 : 0); }
        }

        public PeekStream(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            this.innerStream = stream;
        }

        public override int ReadByte()
        {
            // Have we peeked?  If so, this value still awaits consumption and
            // should be returned.
            if (peeked.HasValue)
            {
                var value = peeked.Value;
                peeked = null;
                return value;
            }

            return innerStream.ReadByte();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int written = 0;

            if (disposed)
            {
                throw new ObjectDisposedException("innerStream");
            }

            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset >= buffer.Length || offset + count > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("offset");
            }

            if (count < 1)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            // If we have a peek value, write it and modify offset, count, and written.
            if (peeked.HasValue)
            {
                buffer[offset++] = peeked.Value;

                count--;
                written++;
                peeked = null;
            }

            if (count > 0)
            {
                written += innerStream.Read(buffer, offset, count);
            }

            return written;
        }

        /// <summary>
        /// Returns the next byte from the <see cref="Stream"/> without
        /// consuming it.
        /// </summary>
        /// <returns></returns>
        public int Peek()
        {
            // Have we peeked since the last consuming read?  If so, the value
            // is unchanged and we should return it.
            if (peeked.HasValue)
            {
                return peeked.Value;
            }

            // Consume the next byte from the inner stream
            var next = innerStream.ReadByte();
            
            // If the stream didn't return EOF, store the peeked value.
            if (next != -1)
            {
                peeked = (byte)next;
            }

            return next;
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose the inner stream, if any
                    if (innerStream != null)
                    {
                        innerStream.Dispose();
                        innerStream = null;
                    }
                }

                this.disposed = true;
            }

            // Don't forget about the base implementation!
            base.Dispose(disposing);
        }

        public override bool CanSeek
        {
            get { return innerStream.CanSeek; }
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
               
            // If we're moving forward in the stream and have peeked, null
            // out the peeked value and subtract one from the offset.
            if (origin != SeekOrigin.End && offset > 0 && peeked.HasValue)
            {
                peeked = null;
                --offset;
            }

            // TODO: Adjust return value
            return innerStream.Seek(offset + (peeked.HasValue ? 1 : 0), origin);
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
