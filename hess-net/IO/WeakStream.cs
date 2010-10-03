using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HessNet.IO
{
    /// <summary>
    /// A thin wrapper around a <see cref="Stream"/> such that when the
    /// <see cref="WeakStream"/> is closed or disposed, the wrapped stream
    /// remains available.
    /// </summary>
    public class WeakStream : Stream
    {
        private Stream inner;

        public WeakStream(Stream inner)
        {
            if (inner == null)
                throw new ArgumentNullException("inner");

            this.inner = inner;
        }

        public override bool CanRead
        {
            get { return inner.CanRead; }
        }

        public override bool CanSeek
        {
            get { return inner.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return inner.CanWrite; }
        }

        public override void Flush()
        {
            inner.Flush();
        }

        public override long Length
        {
            get { return inner.Length; }
        }

        public override long Position
        {
            get
            {
                return inner.Position;
            }
            set
            {
                inner.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return inner.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return inner.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            inner.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            inner.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (inner != null)
                {
                    inner = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}
