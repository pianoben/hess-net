using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using HessNet.IO;

namespace HessNet.Tests
{
    [TestClass]
    public class Utf8StreamTests
    {
        static void ReadText(byte[] encoded, string expected)
        {
            using (var ms = new MemoryStream(encoded))
            {
                var utf8 = new Utf8Stream(ms, false);
                var charbuf = new char[expected.Length];
                var read = utf8.ReadCharacters(charbuf, 0, expected.Length);
                var actual = new string(charbuf);

                Assert.AreEqual(expected.Length, read, "Actual number of characters read differed from expected.");
                Assert.AreEqual(expected, actual, "Decoded string differs from source string.");
            }
        }

        [TestMethod]
        public void TestMethod1()
        {
            
        }

        [TestMethod]
        public void TestReadChar()
        {
            // euro, feh, thorn, om, respectively
            var charBytes = new Dictionary<char, byte[]>
            {
                { '€', new byte[] { 0xE2, 0x82, 0xAC } },
                { 'Ֆ', new byte[] { 0xD5, 0x96 } },
                { 'þ', new byte[] { 0xC3, 0xBE } },
                { 'ॐ', new byte[] { 0xE0, 0xA5, 0x90 } }
            };

            foreach (var kvp in charBytes)
            {
                using (var ms = new MemoryStream(kvp.Value))
                {
                    var utf8 = new Utf8Stream(ms, false);
                    var actual = utf8.ReadCharacter();

                    Assert.AreEqual(kvp.Key, actual);
                }
            }

        }

        [TestMethod]
        public void TestReadMultipleChars()
        {
            var mathLowercaseEl = "abc𝓁";

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(mathLowercaseEl)))
            {
                var charbuf = new char[5];
                var utf8 = new Utf8Stream(ms, false);
                var read = utf8.ReadCharacters(charbuf, 0, 5);

                var str = new string(charbuf);

                Assert.AreEqual(mathLowercaseEl, str);
            }
        }
    }
}
