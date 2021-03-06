﻿using HessNet.Runtime.Serialization.Hessian;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace HessNet.Tests
{
    
    
    /// <summary>
    ///This is a test class for HessianReaderTest and is intended
    ///to contain all HessianReaderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class HessianReaderTest
    {
        private const string loremIpsum = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
        private static readonly byte[] loremIpsumBytes =
        {
            0x4C,0x6F,0x72,0x65,0x6D,0x20,0x69,0x70,0x73,0x75,0x6D,
            0x20,0x64,0x6F,0x6C,0x6F,0x72,0x20,0x73,0x69,0x74,0x20,
            0x61,0x6D,0x65,0x74,0x2C,0x20,0x63,0x6F,0x6E,0x73,0x65,
            0x63,0x74,0x65,0x74,0x75,0x72,0x20,0x61,0x64,0x69,0x70,
            0x69,0x73,0x69,0x63,0x69,0x6E,0x67,0x20,0x65,0x6C,0x69,
            0x74,0x2C,0x20,0x73,0x65,0x64,0x20,0x64,0x6F,0x20,0x65,
            0x69,0x75,0x73,0x6D,0x6F,0x64,0x20,0x74,0x65,0x6D,0x70,
            0x6F,0x72,0x20,0x69,0x6E,0x63,0x69,0x64,0x69,0x64,0x75,
            0x6E,0x74,0x20,0x75,0x74,0x20,0x6C,0x61,0x62,0x6F,0x72,
            0x65,0x20,0x65,0x74,0x20,0x64,0x6F,0x6C,0x6F,0x72,0x65,
            0x20,0x6D,0x61,0x67,0x6E,0x61,0x20,0x61,0x6C,0x69,0x71,
            0x75,0x61,0x2E,0x20,0x55,0x74,0x20,0x65,0x6E,0x69,0x6D,
            0x20,0x61,0x64,0x20,0x6D,0x69,0x6E,0x69,0x6D,0x20,0x76,
            0x65,0x6E,0x69,0x61,0x6D,0x2C,0x20,0x71,0x75,0x69,0x73,
            0x20,0x6E,0x6F,0x73,0x74,0x72,0x75,0x64,0x20,0x65,0x78,
            0x65,0x72,0x63,0x69,0x74,0x61,0x74,0x69,0x6F,0x6E,0x20,
            0x75,0x6C,0x6C,0x61,0x6D,0x63,0x6F,0x20,0x6C,0x61,0x62,
            0x6F,0x72,0x69,0x73,0x20,0x6E,0x69,0x73,0x69,0x20,0x75,
            0x74,0x20,0x61,0x6C,0x69,0x71,0x75,0x69,0x70,0x20,0x65,
            0x78,0x20,0x65,0x61,0x20,0x63,0x6F,0x6D,0x6D,0x6F,0x64,
            0x6F,0x20,0x63,0x6F,0x6E,0x73,0x65,0x71,0x75,0x61,0x74,
            0x2E,0x20,0x44,0x75,0x69,0x73,0x20,0x61,0x75,0x74,0x65,
            0x20,0x69,0x72,0x75,0x72,0x65,0x20,0x64,0x6F,0x6C,0x6F,
            0x72,0x20,0x69,0x6E,0x20,0x72,0x65,0x70,0x72,0x65,0x68,
            0x65,0x6E,0x64,0x65,0x72,0x69,0x74,0x20,0x69,0x6E,0x20,
            0x76,0x6F,0x6C,0x75,0x70,0x74,0x61,0x74,0x65,0x20,0x76,
            0x65,0x6C,0x69,0x74,0x20,0x65,0x73,0x73,0x65,0x20,0x63,
            0x69,0x6C,0x6C,0x75,0x6D,0x20,0x64,0x6F,0x6C,0x6F,0x72,
            0x65,0x20,0x65,0x75,0x20,0x66,0x75,0x67,0x69,0x61,0x74,
            0x20,0x6E,0x75,0x6C,0x6C,0x61,0x20,0x70,0x61,0x72,0x69,
            0x61,0x74,0x75,0x72,0x2E,0x20,0x45,0x78,0x63,0x65,0x70,
            0x74,0x65,0x75,0x72,0x20,0x73,0x69,0x6E,0x74,0x20,0x6F,
            0x63,0x63,0x61,0x65,0x63,0x61,0x74,0x20,0x63,0x75,0x70,
            0x69,0x64,0x61,0x74,0x61,0x74,0x20,0x6E,0x6F,0x6E,0x20,
            0x70,0x72,0x6F,0x69,0x64,0x65,0x6E,0x74,0x2C,0x20,0x73,
            0x75,0x6E,0x74,0x20,0x69,0x6E,0x20,0x63,0x75,0x6C,0x70,
            0x61,0x20,0x71,0x75,0x69,0x20,0x6F,0x66,0x66,0x69,0x63,
            0x69,0x61,0x20,0x64,0x65,0x73,0x65,0x72,0x75,0x6E,0x74,
            0x20,0x6D,0x6F,0x6C,0x6C,0x69,0x74,0x20,0x61,0x6E,0x69,
            0x6D,0x20,0x69,0x64,0x20,0x65,0x73,0x74,0x20,0x6C,0x61,
            0x62,0x6F,0x72,0x75,0x6D,0x2E
        };

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        private static void DoTest<T>(T expected, byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                var reader = new HessianReader(ms, false);
                Assert.AreEqual(expected, reader.ReadObject());
            }
        }

        /// <summary>
        ///A test for ReadDouble
        ///</summary>
        [TestMethod()]
        [DeploymentItem("HessNet.dll")]
        public void ReadDoubleTest()
        {
            var caseNil = new byte[] { 0x67 };
            var caseOne = new byte[] { 0x68 };
            var singleByteZero = new byte[] { 0x69, 0x00 };
            var singleByteTwelve = new byte[] { 0x69, 0x0C };
            var doubleByteZero = new byte[] { 0x6A, 0x00, 0x00 };
            var doubleByteOne = new byte[] { 0x6A, 0x00, 0x01 };
            var doubleByte300 = new byte[] { 0x6A, 0x01, 0x2C };
            var floatZero = new byte[] { 0x6B, 0x00, 0x00, 0x00, 0x00 };
            var floatOne = new byte[] { 0x6B, 0x00, 0x00, 0x80, 0x3F };
            var doubleZero = new byte[] { (byte)'D', 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            var doubleOne = new byte[] { (byte)'D', 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x3F };
            var doubleFour = new byte[] { (byte)'D', 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x40 };
            var doublePi = new byte[] { (byte)'D', 0x18, 0x2D, 0x44, 0x54, 0xFB, 0x21, 0x09, 0x40 };

            DoTest(0d, caseNil);
            DoTest(1d, caseOne);
            DoTest(0d, singleByteZero);
            DoTest(12d, singleByteTwelve);
            DoTest(0d, doubleByteZero);
            DoTest(1d, doubleByteOne);
            DoTest(300d, doubleByte300);
            DoTest(0d, floatZero);
            DoTest(1d, floatOne);
            DoTest(0d, doubleZero);
            DoTest(1d, doubleOne);
            DoTest(4d, doubleFour);
            DoTest(Math.PI, doublePi);
        }

        [TestMethod]
        [DeploymentItem("HessNet.dll")]
        public void ReadStringTest()
        {
            var compactEmpty = new byte[] { 0x00 };
            var compacta = new byte[] { 0x01, (byte)'a' };
            var loremOneChunk = new byte[3 + loremIpsumBytes.Length];
            loremOneChunk[0] = (byte)'S';
            loremOneChunk[1] = 0x01;
            loremOneChunk[2] = 0xBE;
            Array.Copy(loremIpsumBytes, 0, loremOneChunk, 3, loremIpsumBytes.Length);

            var loremChunks = new byte[452];

            loremChunks[0] = (byte)'s';
            loremChunks[1] = 0x00;
            loremChunks[2] = 0xDF;
            loremChunks[226] = (byte)'S';
            loremChunks[227] = 0x00;
            loremChunks[228] = 0xDF;

            Array.Copy(loremIpsumBytes, 0, loremChunks, 3, 223);
            Array.Copy(loremIpsumBytes, 223, loremChunks, 229, 223);

            DoTest(string.Empty, compactEmpty);
            DoTest("a", compacta);
            DoTest(loremIpsum, loremChunks);
        }
    }
}
