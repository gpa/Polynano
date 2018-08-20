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
using Polynano.Persistence.PolygonFileFormat.Common;
using Polynano.Persistence.PolygonFileFormat.Writers;
using System;
using System.Collections.Generic;
using System.IO;

namespace Polynano.Persistence.Tests
{
    [TestFixture]
    public class PlyBinaryWriterTests
    {
        public PlyHeader Header { get; private set; }

        [SetUp]
        public void Setup()
        {
            var elements = new List<PlyElement>()
            {
                new PlyElement("vertex", 2, new List<PlyProperty>()
                {
                    new PlyProperty("x", PlyType.Float),
                    new PlyProperty("y", PlyType.Float),
                    new PlyProperty("z", PlyType.Float)
                }),
                new PlyElement("face", 2, new List<PlyProperty>()
                {
                    new PlyArrayProperty("vertex_index", PlyType.Int, PlyType.Int)
                })
            };

            Header = new PlyHeader(PlyFormat.Ascii, elements);
        }

        [TearDown]
        public void Teardown()
        {
            Header = null;
        }

        [Test]
        public void WritesCorrectlySimpleDecimals()
        {
            var simpleElements = new List<PlyElement>()
            {
                new PlyElement("vertex", 2, new List<PlyProperty>()
                {
                    new PlyProperty("x", PlyType.Float),
                    new PlyProperty("y", PlyType.Float),
                    new PlyProperty("z", PlyType.Float)
                }),
                new PlyElement("face", 2, new List<PlyProperty>()
                {
                    new PlyArrayProperty("vertex_index", PlyType.Int, PlyType.Int)
                })
            };

            var endianess = BitConverter.IsLittleEndian ? PlyFormat.BinaryLittleEndian : PlyFormat.BinaryBigEndian;
            var simpleHeader = new PlyHeader(endianess, simpleElements);
            using (var memoryStream = new MemoryStream())
            {
                using(var writer = new PlyBinaryWriter(memoryStream, simpleHeader))
                {
                    writer.WriteValue(3.0f);
                    writer.WriteValue(5.1113f);
                    writer.WriteValue(-6.14f);

                    writer.WriteValue(3.0f);
                    writer.WriteValue(5.1113f);
                    writer.WriteValue(-6.14f);

                    writer.WriteArray(1, 2, 3);
                    writer.WriteArray(-1, 5, 9);
                }
                using (var reader = new BinaryReader(memoryStream))
                {
                    memoryStream.Seek(-sizeof(int)*8-sizeof(float)*3, SeekOrigin.End);

                    Assert.AreEqual(3.0f, reader.ReadSingle());
                    Assert.AreEqual(5.1113f, reader.ReadSingle());
                    Assert.AreEqual(-6.14f, reader.ReadSingle());

                    Assert.AreEqual(3, reader.ReadInt32());
                    Assert.AreEqual(1, reader.ReadInt32());
                    Assert.AreEqual(2, reader.ReadInt32());
                    Assert.AreEqual(3, reader.ReadInt32());
                    Assert.AreEqual(3, reader.ReadInt32());
                    Assert.AreEqual(-1, reader.ReadInt32());
                    Assert.AreEqual(5, reader.ReadInt32());
                    Assert.AreEqual(9, reader.ReadInt32());
                }
            }
        }
    }
}