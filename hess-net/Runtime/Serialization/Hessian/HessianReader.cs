using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using HessNet.IO;
using HessNet.Text;

namespace HessNet.Runtime.Serialization.Hessian
{
    public class HessianReader : HessianSerializerBase
    {
        private readonly Stack<HessianType> parseStates = new Stack<HessianType>();
        private readonly PeekStream input;
        private readonly bool ownsStream;

        private HessianType CurrentState
        {
            get
            {
                if (parseStates.Count > 0)
                {
                    return parseStates.Peek();
                }

                return HessianType.None;
            }
        }

        public HessianReader(Stream input, bool closeInput)
        {
            this.input = new PeekStream(input);
            this.ownsStream = closeInput;
        }

        public object ReadObject()
        {
            while (true)
            {
                switch (input.Peek())
                {
                    case -1:
                        // -1 indicates EOF.  Do we expect EOF?
                        throw new SerializationException("Unexpected end-of-input.");
                    case 0x00:
                    case 0x01:
                    case 0x02:
                    case 0x03:
                    case 0x04:
                    case 0x05:
                    case 0x06:
                    case 0x07:
                    case 0x08:
                    case 0x09:
                    case 0x0A:
                    case 0x0B:
                    case 0x0C:
                    case 0x0D:
                    case 0x0E:
                    case 0x0F:
                    case 0x10:
                    case 0x11:
                    case 0x12:
                    case 0x13:
                    case 0x14:
                    case 0x15:
                    case 0x16:
                    case 0x17:
                    case 0x18:
                    case 0x19:
                    case 0x1A:
                    case 0x1B:
                    case 0x1C:
                    case 0x1D:
                    case 0x1E:
                    case 0x1F:
                    case 'S':
                    case 's':
                        return ReadString();
                    case 'B':
                        ExitElement();
                        return ReadBinary();
                    case 0x67:
                    case 0x68:
                    case 0x69:
                    case 0x6A:
                    case 0x6B:
                    case 'D':
                        //ExitElement();
                        return ReadDouble();
                    case 'F':
                        ExitElement();
                        return false;
                    case 'I':
                        ExitElement();
                        return ReadInteger();
                    case 0x4A:
                    case 0x4B:
                        ExitElement();
                        return ReadListMapReference();
                    case 'L':
                        ExitElement();
                        return ReadLong();
                    case 'M':
                        ExitElement();
                        return ReadMap();
                    case 'N':
                        ExitElement();
                        return null;
                    case 'O':
                        ReadObjectDefinition();
                        return ReadObject();
                    case 'R':
                        ExitElement();
                        return ReadReference();
                    case 'T':
                        ExitElement();
                        return true;
                    case 'V':
                        ExitElement();
                        return ReadList();
                    case 'X':
                        ExitElement();
                        return ReadXml();
                    case 'b':
                        ExitElement();
                        return ReadBinary();
                    case 'd':
                        ExitElement();
                        return ReadDate();
                }
            }
        }

        private object ReadXml()
        {
            throw new NotImplementedException();
        }

        private string ReadString()
        {
            var sb = new StringBuilder();
            var finalChunk = false;

            do
            {
                int charCount;
                var chunkIndicator = input.ReadByte();

                if (chunkIndicator >= 0x00 && chunkIndicator <= 0x1F)
                {
                    finalChunk = true;
                    charCount = chunkIndicator;
                }
                else if (chunkIndicator == 'S' || chunkIndicator == 's')
                {
                    finalChunk = chunkIndicator == 'S';
                    charCount = ReadLength();
                }
                else
                {
                    throw new SerializationException("Invalid Hessian 2.0 string.");
                }

                if (charCount == 0)
                {
                    // We have no further work to do in this chunk;
                    // read the next one.  If another chunk exists, this one
                    // is malformed.  Most likely, this chunk is the first and
                    // only chunk of an empty string.  Either way, I don't
                    // think that we care.
                    continue;
                }

                using (var stringstream = new Utf8Stream(input))
                {
                    var charbuf = new char[charCount];
                    var charsRead = stringstream.ReadCharacters(charbuf, 0, charCount);

                    if (charsRead != charCount)
                        throw new SerializationException(string.Format("Invalid string chunk - expected {0} characters, found {1} characters.", charCount, charsRead));

                    sb.Append(charbuf);
                }
                /*
                var converter = new Utf8StreamConverter(input);

                for (var i = 0; i < charCount; ++i)
                {
                    sb.Append(converter.ReadCharacter());
                }*/
            }
            while (!finalChunk);

            return sb.ToString();
        }

        private object ReadList()
        {
            throw new NotImplementedException();
        }

        private object ReadListMapReference()
        {
            throw new NotImplementedException();
        }

        private void ReadObjectDefinition()
        {
            throw new NotImplementedException();
        }

        private object ReadLong()
        {
            throw new NotImplementedException();
        }

        private object ReadReference()
        {
            throw new NotImplementedException();
        }

        private object ReadMap()
        {
            throw new NotImplementedException();
        }

        private object ReadDate()
        {
            throw new NotImplementedException();
        }

        private object ReadBinary()
        {
            throw new NotImplementedException();
        }

        private int ReadInteger()
        {
            var b = (byte)input.ReadByte();

            if (b >= 0x80 && b <= 0xBF)
            {
                // Compact one-byte integer between -16 and 47
                return b - 0x90;
            }
            else if (b >= 0xC0 && b <= 0xCF)
            {
                // Compact two-byte integer between -2048 and 2047
                var lowByte = input.ReadByte();

                if (lowByte == -1)
                    goto Error;

                // Formula per Hessian 2.0 spec, revision 2
                return 256 * (b - 0xC8) + lowByte;
            }
            else if (b >= 0xD0 && b <= 0xDF)
            {
                // Compact three-byte integer between -262144 and 262143
                var middleByte = input.ReadByte();
                var lowByte = input.ReadByte();

                if (middleByte == -1 || lowByte == -1)
                    goto Error;

                // Formula per Hessian 2.0 spec, revision 2
                return ((b - 0xD4) << 16) + (middleByte << 8) + lowByte;
            }
            else if (b == (byte)'I')
            {
                // Standard four-byte integer
                var buffer = new byte[4];
                var bytesRead = input.Read(buffer, 0, 4);

                if (bytesRead != 4)
                    goto Error;

                return (buffer[0] << 24) + (buffer[1] << 16) + (buffer[2] << 8) + buffer[3];
            }

        Error:
            throw new SerializationException("Hessian 2.0 integer expected.");
        }

        private double ReadDouble()
        {
            byte[] buffer;

            switch (input.Peek())
            {
                case 0x67:
                    // Byte 0x67 indicates that the encoded double was 0.0.
                    // No further interpretation is necessary.
                    return 0.0;
                case 0x68:
                    // Byte 0x68 indicates that the encoded double was 1.0.
                    // No further interpretation is necessary.
                    return 1.0;
                case 0x69:
                    // In this case, the double encoded had no fractional
                    // component and was between -128 and 127.  Consequently,
                    // it was encoded as a single byte.  This byte can be cast
                    // to a double and returned as-is.

                    // Discard indicator byte
                    input.ReadByte();

                    var number = input.ReadByte();
                    return (double)number;
                case 0x6A:
                    // In this case, the double encoded had no fractional
                    // component and was between -32768.0 and 32767.0.  It can
                    // be decoded by reading the two bytes, converting them to
                    // a short, and casting that value to a double.

                    // Discard indicator byte
                    input.ReadByte();

                    // Read two bytes
                    var highByte = input.ReadByte();
                    var lowByte = input.ReadByte();

                    if (highByte == -1 || lowByte == -1)
                    {
                        throw new EndOfStreamException();
                    }

                    // Make a short out of the two bytes, cast to a double,
                    // and return.
                    return (double)(short)((highByte << 8) + lowByte);
                case 0x6B:
                    // In this case, the double encoded was equivalent to its
                    // 32-bit float representation.  To decode, convert the 4
                    // bytes to a float and then cast that value to a double.

                    // Discard indicator byte
                    input.ReadByte();

                    // Read four bytes
                    buffer = new byte[4];

                    if (input.Read(buffer, 0, 4) != 4)
                    {
                        throw new EndOfStreamException();
                    }

                    // Convert the four bytes to a float
                    var value = BitConverter.ToSingle(buffer, 0);

                    // Cast the float to a double and return it.
                    return (double)value;
                default:
                    // In this case, no compact double encoding was detected.
                    // We need to first validate that the required byte 'D' is
                    // present, and then read the eight bytes comprising the
                    // double.

                    // Ensure that 'D' is the next byte.
                    AssertNextByte((byte)'D');

                    // Read 8-byte double value
                    buffer = new byte[8];

                    if (input.Read(buffer, 0, 8) != 8)
                    {
                        throw new EndOfStreamException();
                    }

                    // Convert the eight bytes to a double and return it.
                    return BitConverter.ToDouble(buffer, 0);
            }
        }

        private void EnterElement(HessianType element)
        {
            parseStates.Push(element);
        }

        private void ExitElement()
        {
            if (parseStates.Count < 1)
                throw new SerializationException("WTF");

            parseStates.Pop();
        }

        private ushort ReadLength()
        {
            var highByte = input.ReadByte();
            var lowByte = input.ReadByte();

            if (highByte == -1 || lowByte == -1)
            {
                throw new SerializationException("Invalid Hessian 2.0 length.");
            }

            return (ushort)((highByte << 8) + lowByte);
        }

        private void AssertNextByte(byte expected)
        {
            if (input.ReadByte() != expected)
            {
                var message = string.Format("Error reading value; expected {0}, but found {1}", expected, input.Peek());
                throw new SerializationException(message);
            }
        }
    }
}
