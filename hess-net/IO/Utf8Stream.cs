using System;
using System.IO;
using System.Text;

using HessNet.Text;

namespace HessNet.IO
{
    public class Utf8Stream : Stream
    {
        private bool disposed;
        private bool ownsStream;
        private int lastCharacterPosition;
        private Stream source;
        private byte[] bytebuf;
        private char[] charbuf;
        private bool canReadMultipleChars;
        private int? nextCodePoint;

        public Utf8Stream(Stream source)
            : this(source, false)
        {
            
        }

        public Utf8Stream(Stream source, bool ownsStream)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            this.source = source;
            this.bytebuf = new byte[4];
            this.charbuf = new char[1];
            this.canReadMultipleChars = source.CanSeek;
            this.ownsStream = ownsStream;
        }

        public int ReadCharacters(char[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            if (count < 0)
                throw new ArgumentOutOfRangeException("count");

            if (offset < 0 || offset + count > buffer.Length)
                throw new ArgumentOutOfRangeException("offset");

            if (count == 0)
                return 0;

            return canReadMultipleChars
                ? ReadMultipleCharacters(buffer, offset, count)
                : ReadOneAtATime(buffer, offset, count);
        }

        public char ReadCharacter()
        {
            if (ReadMultipleCharacters(charbuf, 0, 1) == 0)
                throw new InvalidUnicodeException();

            return charbuf[0];
        }

        private int ReadMultipleCharacters(char[] buffer, int offset, int count)
        {
            // Read requested count * four bytes from the source stream
            var charsRead = 0;
            var bytes = new byte[count * 4]; // Max char in UTF-8 is four bytes long; this is the longest bytestring we could possibly need.
            var bytesRead = source.Read(bytes, 0, bytes.Length);

            int bytesConsumed;

            // Convert 
            charsRead = Convert(bytes, 0, buffer, offset, count, out bytesConsumed);

            // 
            source.Seek(bytesConsumed - bytesRead, SeekOrigin.Current);

            return charsRead;
        }

        private int ReadOneAtATime(char[] buffer, int offset, int count)
        {
            var bytesUsed = 0;
            var charsRead = 0;

            while (charsRead < count)
            {
                var endOfStream = false;
                int value;

                for (var i = 0; i < 4; ++i)
                {
                    value = source.ReadByte();

                    if (value == -1)
                    {
                        endOfStream = true;
                        break;
                    }

                    bytebuf[i] = (byte)value;

                    if (Convert(bytebuf, 0, buffer, charsRead + offset, 1, out bytesUsed) == 1)
                    {
                        ++charsRead;
                        break;
                    }

                    if (i == 3)
                    {
                        // If we're here, we've failed to read a UTF-8 character.
                        // Assign the unknown character, clear the buffer, and
                        // keep going.

                        bytebuf[0] = 0;
                        bytebuf[1] = 0;
                        bytebuf[2] = 0;
                        bytebuf[3] = 0;

                        buffer[offset + (charsRead++)] = (char)0xFFFE;
                    }
                }

                if (endOfStream)
                    break;
            }

            return charsRead;
        }

        private unsafe int Convert(byte[] data, int dataOffset, char[] buffer, int charOffset, int count, out int bytesUsed)
        {
            bytesUsed = 0;
            var charCount = 0;

            fixed (byte* buf = &(data[dataOffset]))
            {
                // determine max number of chars
                var end = &(buf[data.Length]);
                var b = buf;

                // If our buffer begins in the middle of a multibyte char, skip
                // it and move to the beginning of the next char.
                while ((*b & 0xC0) == 0x80 && b != end)
                {
                    ++b;
                    ++bytesUsed;
                }

                while (b != end && charCount < count)
                {
                    int charBytes;
                    var codepoint = 0xFFFD; // Unicode 'replacement' character

                    if (nextCodePoint != null)
                    {
                        codepoint = nextCodePoint.Value;
                        nextCodePoint = null;
                        charBytes = 0;
                        goto DoOutput;
                    }

                    if (*b < 0x80)
                    {
                        // One-byte encoding
                        codepoint = *b;
                        charBytes = 1;
                    }
                    else if ((*b & 0xE0) == 0xC0)
                    {
                        // Two-byte encoding
                        charBytes = 2;

                        // Do we have enough data for this?
                        if (b + 1 >= end)
                        {
                            break;
                        }

                        // Is this a legal two-byte UTF-8 char?
                        if (*b < 0xC2 ||
                            *b > 0xDF ||
                            *(b + 1) < 0x80 ||
                            *(b + 1) > 0xBF)
                        {
                            // This character encoding is illegal - write a null char and continue;
                            goto DoOutput;
                        }

                        codepoint = ((*b & 0x1F) << 6) | (*(b + 1) & 0x3F);
                    }
                    else if ((*b & 0xF0) == 0xE0)
                    {
                        // Three-byte encoding
                        charBytes = 3;

                        // Do we have enough data for this?
                        if (b + 2 >= end)
                        {
                            break;
                        }

                        // Is this a legal three-byte UTF-8 char?
                        if (*(b + 1) < (*b == 0xE0 ? 0xA0 : 0x80) ||
                            *(b + 1) > 0xBF ||
                            *(b + 2) < 0x80 ||
                            *(b + 2) > 0xBF)
                        {
                            // Nope.
                            goto DoOutput;
                        }
                        // 00001001 01010000    11100000      10100101     10010000
                        //                      0xE0          
                        // Scalar value         b0            b1           b2
                        // zzzzyyyy yyxxxxxx    1110zzzz      10yyyyyy     10xxxxxx
                        codepoint = ((*b & 0x0F) << 12)
                                  | ((*(b + 1) & 0x3F) << 6)
                                  | (*(b + 2) & 0x3F);
                    }
                    else if ((*b & 0xF8) == 0xF0)
                    {
                        // Four-byte encoding
                        charBytes = 4;

                        // Do we have enough data?
                        if (b + 3 >= end)
                        {
                            break;
                        }

                        // Is this a legal four-byte UTF-8 char?
                        if (*(b + 1) < (*b == 0xF0 ? 0x90 : 0x80) ||
                            *(b + 1) > (*b == 0xF4 ? 0x8F : 0xBF) ||
                            *(b + 2) < 0x80 ||
                            *(b + 2) > 0xBF ||
                            *(b + 3) < 0x80 ||
                            *(b + 3) > 0xBF)
                        {
                            goto DoOutput;
                        }

                        // Scalar value                  b0           b1           b2           b3
                        // 000uuuuu zzzzyyyy yyxxxxxx    11110uuu     10uuzzzz     10yyyyyy     10xxxxxx
                        codepoint = ((*b & 0x07) << 18)
                                  | ((*(b + 1) & 0x3F) << 12)
                                  | ((*(b + 2) & 0x3F) << 6)
                                  | (*(b + 3) & 0x3F);
                    }
                    else
                    {
                        // Illegal UTF-8 opening byte - skip it.
                        charBytes = 1;
                    }

                DoOutput:
                    if (codepoint > 0xFFFF)
                    {
                        // Write UTF-16 surrogate pairs.
                        // Algorithm taken directly from Unicode site.
                        // http://www.unicode.org/faq/utf_bom.html#utf16-4
                        const int LEAD_OFFSET = 0xD800 - (0x10000 >> 10);

                        buffer[charOffset + charCount++] = (char)(LEAD_OFFSET + (codepoint >> 10));

                        // Save the low-surrogate value, in case the user has requested
                        // n characters and the low surrogate is n + 1.  They'll get it
                        // at the next read.
                        nextCodePoint = (0xDC00 + (codepoint & 0x3FF));
                    }
                    else
                    {
                        buffer[charOffset + charCount++] = (char)codepoint;
                    }

                    b += charBytes;
                    bytesUsed += charBytes;
                }
            }

            return charCount;
        }

        #region Stream overrides

        public override bool CanRead
        {
            get { return source.CanRead; }
        }

        public override bool CanSeek
        {
            get { return source.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return source.CanWrite; }
        }

        public override bool CanTimeout
        {
            get { return source.CanTimeout; }
        }

        public override void Flush()
        {
            source.Flush();
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return source.BeginRead(buffer, offset, count, callback, state);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return source.EndRead(asyncResult);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return source.BeginWrite(buffer, offset, count, callback, state);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            source.EndWrite(asyncResult);
        }

        public override void Close()
        {
            if (ownsStream)
            {
                source.Close();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (source != null)
                    {
                        // Only dispose the stream if the user specifically
                        // indicates that we should.
                        if (ownsStream)
                        {
                            source.Dispose();
                        }

                        // Get rid of the reference regardless.
                        source = null;
                    }
                }

                disposed = true;
            }

            base.Dispose(disposing);
        }

        public override int ReadByte()
        {
            return source.ReadByte();
        }

        public override void WriteByte(byte value)
        {
            source.WriteByte(value);
        }

        public override int ReadTimeout
        {
            get { return source.ReadTimeout; }
            set { source.ReadTimeout = value; }
        }

        public override int WriteTimeout
        {
            get { return source.WriteTimeout; }
            set { source.WriteTimeout = value; }
        }

        public override long Length
        {
            get { return source.Length; }
        }

        public override long Position
        {
            get { return source.Position; }
            set { source.Position = value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return source.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return source.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            source.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            source.Write(buffer, offset, count);
        }

        #endregion Stream overrides
    }
}
