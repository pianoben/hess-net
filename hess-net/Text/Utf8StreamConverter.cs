using System;
using System.IO;
using System.Text;

namespace HessNet.Text
{
    /// <summary>
    /// Reads UTF-8 characters from a <see cref="Stream"/> one at a time,
    /// without consuming large chunks of the stream data like a
    /// <see cref="StreamReader"/>.
    /// </summary>
    public class Utf8StreamConverter : IDisposable
    {
        private Stream input;
        private static readonly Decoder decoder = Encoding.UTF8.GetDecoder();
        private byte[] bytebuf;
        private char[] charbuf;

        public Utf8StreamConverter(Stream input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            this.input = input;
            this.bytebuf = new byte[4];
            this.charbuf = new char[1];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="InvalidUnicodeException"/>
        /// <exception cref="EndOfStreamException"/>
        /// <returns></returns>
        public char ReadCharacter()
        {
            // Read the first byte
            var b0 = ReadByte();

            // If the first byte is less than 0x80, it's ASCII and can be
            // returned as-is.
            if (b0 < 0x80)
            {
                return (char)b0;
            }
            else if ((b0 & 0xC0) == 0xC0)
            {
                var b1 = ReadByte();

                bytebuf[0] = b0;
                bytebuf[1] = b1;

                return DecodeChar(2);
            }
            else if ((b0 & 0xE0) == 0xE0)
            {
                bytebuf[0] = b0;

                // Read two more bytes
                ReadBytes(1, 2);

                return DecodeChar(3);
            }
            else if ((b0 & 0xF0) == 0xF0)
            {
                bytebuf[0] = b0;

                // Read three more bytes
                ReadBytes(1, 3);

                return DecodeChar(4);
            }

            throw new InvalidUnicodeException();
        }

        public void Dispose()
        {
            input = null;
            GC.SuppressFinalize(this);
        }

        private unsafe char DecodeChar(int byteCount)
        {
            fixed (byte* buf = bytebuf)
            {
                char* c = stackalloc char[1];

                if (decoder.GetChars(buf, byteCount, c, 1, true) != 1)
                {
                    throw new InvalidUnicodeException();
                }

                return *c;
            }
        }

        private byte ReadByte()
        {
            var b = input.ReadByte();

            if (b < 0)
                throw new EndOfStreamException();

            return (byte)b;
        }

        private void ReadBytes(int offset, int byteCount)
        {
            if (input.Read(bytebuf, offset, byteCount) != byteCount)
            {
                throw new EndOfStreamException();
            }
        }
    }
}
