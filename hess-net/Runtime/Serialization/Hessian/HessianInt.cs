using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace HessNet.Runtime.Serialization.Hessian
{
    public class HessianInt : HessianToken
    {
        private int value;

        public override IEnumerable<byte> GetBytes()
        {
            if (value >= -16 && value <= 47)
            {
                yield return (byte)(value - 0x90);
            }
            else if (value >= -2048 && value <= 2047)
            {
                yield return (byte)((value >> 8) + 0xC8);
                yield return (byte)(value);
            }
            else if (value >= -262144 && value <= 262143)
            {
                yield return (byte)((value >> 16) + 0xD4);
                yield return (byte)(value >> 8);
                yield return (byte)(value);
            }
            else
            {
                yield return (byte)'I';
                yield return (byte)(value >> 24);
                yield return (byte)(value >> 16);
                yield return (byte)(value >> 8);
                yield return (byte)(value);
            }
        }

        public override void Read(Stream source)
        {
            var firstByte = source.ReadByte();

            CheckEndOfStream(firstByte);

            if (firstByte == 'I')
            {
                // Full four-byte integer
                var bytes = new byte[4];
                var read = source.Read(bytes, 0, 4);

                if (read != 4)
                {
                    throw new SerializationException("Invalid Hessian 2.0 int - expected four bytes, found " + read);
                }

                value = (bytes[0] << 24)
                      + (bytes[1] << 16)
                      + (bytes[2] << 8)
                      + (bytes[3]);
            }
            else if (firstByte >= 0x80 && firstByte <= 0xBF)
            {
                // Compact one-byte integer
                value = firstByte - 0x90;
            }
            else if (firstByte >= 0xC0 && firstByte <= 0xCF)
            {
                // Compact two-byte integer
                var secondByte = source.ReadByte();

                CheckEndOfStream(secondByte);

                value = ((firstByte - 0xC8) << 8) + secondByte;
            }
            else if (firstByte >= 0xD0 && firstByte <= 0xDF)
            {
                // Compact three-byte integer
                var secondByte = source.ReadByte();
                var thirdByte = source.ReadByte();

                CheckEndOfStream(secondByte);
                CheckEndOfStream(thirdByte);

                value = ((firstByte - 0xD4) << 16) + (secondByte << 8) + thirdByte;
            }
        }
    }
}
