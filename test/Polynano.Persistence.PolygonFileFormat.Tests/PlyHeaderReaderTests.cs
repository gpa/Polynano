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
using Polynano.Persistence.PolygonFileFormat.Readers;
using Polynano.Persistence.PolygonFileFormat.Readers.StreamReaders;
using System;
using System.IO;
using System.Text;

namespace Polynano.Persistence.Tests
{

    [TestFixture]
    public class PlyHeaderReaderTests
    {
        private const string FullyFeaturedValidHeader = @"ply
format ascii 1.0
comment model
comment v1
obj_info scale=1.0
obj_info author=admin
element vertex 3
property char p1
property double p2
property float p3
property int p4
property short p5
property uchar p6
property uint p7
property ushort p8
element face 100000000
property list char int p1
property list int int p2
property list short int p3
property list uchar int p4
property list uint int p5
property list ushort int p6
end_header
";

        [Test]
        public void Parse_ParsesBasicHeaderInfoCorrectly()
        {
            var stream = GetStream(FullyFeaturedValidHeader);
            var bufferedStreamReader = new BufferedStreamReader(stream);
            var headerParser = new PlyHeaderReader(bufferedStreamReader);
            var header = headerParser.Read();
            Assert.AreEqual(PlyFormat.Ascii, header.Format);
            Assert.AreEqual("model\r\nv1", header.Comment);
            Assert.AreEqual("scale=1.0\r\nauthor=admin", header.ObjectInfo);
        }

        [Test]
        public void Parse_ParsesElementsCorrectly()
        {
            var stream = GetStream(FullyFeaturedValidHeader);
            var bufferedStreamReader = new BufferedStreamReader(stream);
            var headerParser = new PlyHeaderReader(bufferedStreamReader);
            var header = headerParser.Read();
            var elements = header.Elements;
            Assert.AreEqual(2, elements.Count);
            Assert.AreEqual("vertex", elements[0].Name);
            Assert.AreEqual(3, elements[0].InstanceCount);
            Assert.AreEqual("face", elements[1].Name);
            Assert.AreEqual(100000000, elements[1].InstanceCount);
        }

        [Test]
        public void Parse_ParsesPropertiesCorrectly()
        {
            var stream = GetStream(FullyFeaturedValidHeader);
            var bufferedStreamReader = new BufferedStreamReader(stream);
            var headerParser = new PlyHeaderReader(bufferedStreamReader);
            var header = headerParser.Read();

            var properties = header.Elements[0].Properties;
            Assert.AreEqual("p1", properties[0].Name);
            Assert.AreEqual("p2", properties[1].Name);
            Assert.AreEqual("p3", properties[2].Name);
            Assert.AreEqual("p4", properties[3].Name);
            Assert.AreEqual("p5", properties[4].Name);
            Assert.AreEqual("p6", properties[5].Name);
            Assert.AreEqual("p7", properties[6].Name);
            Assert.AreEqual("p8", properties[7].Name);
            Assert.AreEqual(PlyType.Char, properties[0].ValueType);
            Assert.AreEqual(PlyType.Double, properties[1].ValueType);
            Assert.AreEqual(PlyType.Float, properties[2].ValueType);
            Assert.AreEqual(PlyType.Int, properties[3].ValueType);
            Assert.AreEqual(PlyType.Short, properties[4].ValueType);
            Assert.AreEqual(PlyType.Uchar, properties[5].ValueType);
            Assert.AreEqual(PlyType.Uint, properties[6].ValueType);
            Assert.AreEqual(PlyType.Ushort, properties[7].ValueType);
        }

        [Test]
        public void Parse_ParsesListPropertiesCorrectly()
        {
            var stream = GetStream(FullyFeaturedValidHeader);
            var bufferedStreamReader = new BufferedStreamReader(stream);
            var headerParser = new PlyHeaderReader(bufferedStreamReader);
            var header = headerParser.Read();

            var properties = header.Elements[1].Properties;
            Assert.AreEqual("p1", properties[0].Name);
            Assert.AreEqual("p2", properties[1].Name);
            Assert.AreEqual("p3", properties[2].Name);
            Assert.AreEqual("p4", properties[3].Name);
            Assert.AreEqual("p5", properties[4].Name);
            Assert.AreEqual("p6", properties[5].Name);

            Action<int> testType = (int i) => {
                var arrayProperty = properties[i] as PlyArrayProperty;
                Assert.NotNull(arrayProperty);
            };
            Assert.AreEqual(PlyType.Char, ((PlyArrayProperty)properties[0]).ArraySizeType);
            Assert.AreEqual(PlyType.Int, ((PlyArrayProperty)properties[1]).ArraySizeType);
            Assert.AreEqual(PlyType.Short, ((PlyArrayProperty)properties[2]).ArraySizeType);
            Assert.AreEqual(PlyType.Uchar, ((PlyArrayProperty)properties[3]).ArraySizeType);
            Assert.AreEqual(PlyType.Uint, ((PlyArrayProperty)properties[4]).ArraySizeType);
            Assert.AreEqual(PlyType.Ushort, ((PlyArrayProperty)properties[5]).ArraySizeType);
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