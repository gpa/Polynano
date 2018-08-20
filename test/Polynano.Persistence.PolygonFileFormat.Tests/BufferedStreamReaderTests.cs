/*
MIT License

Copyright(c) 2018 Gratian Pawliszyn

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using NUnit.Framework;
using Polynano.Persistence.PolygonFileFormat.Readers.StreamReaders;
using System;
using System.IO;
using System.Text;

namespace Polynano.Persistence.Tests
{
    [TestFixture]
    public class BufferedStreamReaderTests
    {
        [Test]
        public void ReadLine_ReadsSimpleLines()
        {
            var header = @"line1 a
            line2 b";

            var stream = GetStream(header);
            var reader = new BufferedStreamReader(stream);
            Assert.AreEqual("line1 a", reader.ReadLine());
            Assert.AreEqual("line2 b", reader.ReadLine());
            Assert.AreEqual(null, reader.ReadLine());
        }

        [Test]
        public void ReadLine_ReadsSimpleLinesAndSwitchesBetweenBuffers()
        {
            var line1 = "linelinelinelinelinelinelinelinelinelineline1";
            var line2 = "linelinelinelinelinelinelinelinelinelineline2";
            var line3 = "linelinelinelinelinelinelinelinelinelineline3";
            var line4 = "linelinelinelinelinelinelinelinelinelineline4";
            var line5 = "linelinelinelinelinelinelinelinelinelineline5";
            var bufferContent = $@"{line1}
            {line2}
            {line3}
            {line4}
            {line5}";

            var stream = GetStream(bufferContent);
            var reader = new BufferedStreamReader(stream, 5);
            Assert.AreEqual(line1, reader.ReadLine());
            Assert.AreEqual(line2, reader.ReadLine());
            Assert.AreEqual(line3, reader.ReadLine());
            Assert.AreEqual(line4, reader.ReadLine());
            Assert.AreEqual(line5, reader.ReadLine());
            Assert.AreEqual(null, reader.ReadLine());
        }
        
        [Test]
        public void ReadLine_SkipsMultipleNewlines()
        {
            var line1 = "line1";
            var line2 = "line2";
            var line3 = "line3";
            var bufferContent = $@"{line1}

            {line2}

            {line3}";

            var stream = GetStream(bufferContent);
            var reader = new BufferedStreamReader(stream, 5);
            Assert.AreEqual(line1, reader.ReadLine());
            Assert.AreEqual(line2, reader.ReadLine());
            Assert.AreEqual(line3, reader.ReadLine());
            Assert.AreEqual(null, reader.ReadLine());
        }

        [Test]
        public void ReadToken_ReadsSimpleTokens()
        {
            var line1 = "token1 token2 token3 token4 token_5";
            var line2 = "token6 token7";
            var line3 = "token8";
            var bufferContent = $@"{line1}
            {line2}
            {line3}";

            var stream = GetStream(bufferContent);
            var reader = new BufferedStreamReader(stream, 5);
            Assert.AreEqual("token1", reader.ReadToken());
            Assert.AreEqual("token2", reader.ReadToken());
            Assert.AreEqual("token3", reader.ReadToken());
            Assert.AreEqual("token4", reader.ReadToken());
            Assert.AreEqual("token_5", reader.ReadToken());
            Assert.AreEqual("token6", reader.ReadToken());
            Assert.AreEqual("token7", reader.ReadToken());
            Assert.AreEqual("token8", reader.ReadToken());
            Assert.AreEqual(null, reader.ReadLine());
        }

        [Test]
        public void ReadBytes_ReadsSimpleBytes()
        {
            var memoryStream = new MemoryStream();
            var binaryWriter = new System.IO.BinaryWriter(memoryStream);
            binaryWriter.Write((uint)11);
            binaryWriter.Write(-12.0f);
            binaryWriter.Write(3.1112f);
            binaryWriter.Write((int)0);
            binaryWriter.Write((Int16)16);
            binaryWriter.Write(((short)3));
            binaryWriter.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);
            var reader = new BufferedStreamReader(memoryStream);
            Assert.AreEqual(BitConverter.GetBytes((uint)11), reader.ReadBytes(sizeof(uint)));
            Assert.AreEqual(BitConverter.GetBytes(-12.0f), reader.ReadBytes(sizeof(float)));
            Assert.AreEqual(BitConverter.GetBytes(3.1112f), reader.ReadBytes(sizeof(float)));
            Assert.AreEqual(BitConverter.GetBytes((int)0), reader.ReadBytes(sizeof(int)));
            Assert.AreEqual(BitConverter.GetBytes((Int16)16), reader.ReadBytes(sizeof(Int16)));
            Assert.AreEqual(BitConverter.GetBytes((short)3), reader.ReadBytes(sizeof(short)));
        }

        [Test]
        public void ReadBytes_ReadsSimpleBytesAndGoesThroughMultipleBuffers()
        {
            var memoryStream = new MemoryStream();
            var binaryWriter = new System.IO.BinaryWriter(memoryStream);
            binaryWriter.Write((Int16)16);
            binaryWriter.Write((uint)11);
            binaryWriter.Write(-12.0f);
            binaryWriter.Write(3.1112f);
            binaryWriter.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);
            var reader = new BufferedStreamReader(memoryStream, 4);
            Assert.AreEqual(BitConverter.GetBytes((Int16)16), reader.ReadBytes(sizeof(Int16)));
            Assert.AreEqual(BitConverter.GetBytes((uint)11), reader.ReadBytes(sizeof(uint)));
            Assert.AreEqual(BitConverter.GetBytes(-12.0f), reader.ReadBytes(sizeof(float)));
            Assert.AreEqual(BitConverter.GetBytes(3.1112f), reader.ReadBytes(sizeof(float)));
        }

        private MemoryStream GetStream(string content)
        {
            MemoryStream ms = new MemoryStream();
            ASCIIEncoding uniEncoding = new ASCIIEncoding();
            var sw = new StreamWriter(ms, uniEncoding);
            sw.Write(content);
            sw.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
    }
}