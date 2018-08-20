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
using System.Collections.Generic;

namespace Polynano.Persistence.PolygonFileFormat.Tests
{
    [TestFixture]
    public class PlyHeaderWriterTests
    {
        public PlyHeader ValidHeader { get; private set; }

        [SetUp]
        public void Setup()
        {
            var elements = new List<PlyElement>()
            {
                new PlyElement("vertex", 3, new List<PlyProperty>()
                {
                    new PlyProperty("p1", PlyType.Char),
                    new PlyProperty("p2", PlyType.Double),
                    new PlyProperty("p3", PlyType.Float),
                    new PlyProperty("p4", PlyType.Int),
                    new PlyProperty("p5", PlyType.Short),
                    new PlyProperty("p6", PlyType.Uchar),
                    new PlyProperty("p7", PlyType.Uint),
                    new PlyProperty("p8", PlyType.Ushort),
                }),
                new PlyElement("face", 100000000, new List<PlyProperty>()
                {
                    new PlyArrayProperty("p1", PlyType.Char, PlyType.Int),
                    new PlyArrayProperty("p2", PlyType.Int, PlyType.Int),
                    new PlyArrayProperty("p3", PlyType.Short, PlyType.Int),
                    new PlyArrayProperty("p4", PlyType.Uchar, PlyType.Int),
                    new PlyArrayProperty("p5", PlyType.Uint, PlyType.Int),
                    new PlyArrayProperty("p6", PlyType.Ushort, PlyType.Int),
                })
            };

            ValidHeader = new PlyHeader(PlyFormat.Ascii, "Comment", null, elements);
        }

        [TearDown]
        public void Teardown()
        {
            ValidHeader = null;
        }
     
        [Test]
        public void GetHeader_GeneratesValidHeader()
        {
            var expected = @"ply
format ascii 1.0
comment Comment
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
            var actual = PlyHeaderWriter.GetHeader(ValidHeader);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetHeader_GeneratesValidHeaderWithNoComment()
        {
            var expected = @"ply
format ascii 1.0
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
            ValidHeader.Comment = null;
            var actual = PlyHeaderWriter.GetHeader(ValidHeader);

            Assert.AreEqual(expected, actual);
        }
        
        [Test]
        public void GetHeader_GeneratesValidHeaderWithObjectInfo()
        {
            var expected = @"ply
format ascii 1.0
obj_info scale=1.0
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
            ValidHeader.Comment = null;
            ValidHeader.ObjectInfo = "scale=1.0";
            var actual = PlyHeaderWriter.GetHeader(ValidHeader);

            Assert.AreEqual(expected, actual);
        }
            
        [Test]
        public void GetHeader_GeneratesValidHeaderEscapesNewLines()
        {
            var expected = @"ply
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
            ValidHeader.Comment = "model\nv1";
            ValidHeader.ObjectInfo = "scale=1.0\r\nauthor=admin";
            var actual = PlyHeaderWriter.GetHeader(ValidHeader);

            Assert.AreEqual(expected, actual);
        }
    }
}