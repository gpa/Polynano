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
using Polynano.Persistence.PolygonFileFormat.Exceptions;
using Polynano.Persistence.PolygonFileFormat.Writers;
using System;
using System.Collections.Generic;
using System.IO;

namespace Polynano.Persistence.Tests
{
    [TestFixture]
    public class PlyAsciiWriterTests
    {
        [Test]
        public void ThrowsWhenTooManyValuesWritten()
        {
            var elements = new List<PlyElement>()
            {
                new PlyElement("vertex", 1, new List<PlyProperty>()
                {
                    new PlyProperty("x", PlyType.Float),
                }),
                new PlyElement("face", 1, new List<PlyProperty>()
                {
                    new PlyArrayProperty("vertex_index", PlyType.Int, PlyType.Int)
                })
            };
            
            var header = new PlyHeader(PlyFormat.Ascii, elements);
            using (var stream = new MemoryStream())
            {
                using(var writer = new PlyAsciiWriter(stream, header))
                {
                    writer.WriteValue(1.0f);
                    writer.WriteArray(new int[] { 12, 16, 45});
                    Assert.Throws<InvalidOperationException>(() => writer.WriteArray(new int[] { 1, 2, 3}));
                }
            }
        }

        [Test]
        public void ThrowsOnArrayWriteWhenValueExpected()
        {
            var elements = new List<PlyElement>()
            {
                new PlyElement("vertex", 1, new List<PlyProperty>()
                {
                    new PlyProperty("x", PlyType.Float),
                }),
                new PlyElement("face", 1, new List<PlyProperty>()
                {
                    new PlyArrayProperty("vertex_index", PlyType.Int, PlyType.Int)
                })
            };
            
            var header = new PlyHeader(PlyFormat.Ascii, elements);
            using (var stream = new MemoryStream())
            {
                var writer = new PlyAsciiWriter(stream, header);
                Assert.Throws<PlyWriteArrayWhenValueExpectedException>(() => writer.WriteArray(1, 2, 3));
                Assert.DoesNotThrow(() => writer.WriteValue(1.2f));
                Assert.Throws<PlyWriteValueWhenArrayExpectedException>(() => writer.WriteValue(12.0f));
                Assert.Throws<PlyIterationNotFinishedException>(() => writer.Dispose());
                Assert.DoesNotThrow(() => writer.WriteArray(1, 2, 3));
                Assert.DoesNotThrow(() => writer.Dispose());
            }
        }

        [Test]
        public void ThrowsOnUnexpectedDataType()
        {
            var elements = new List<PlyElement>()
            {
                new PlyElement("vertex", 1, new List<PlyProperty>()
                {
                    new PlyProperty("x", PlyType.Float),
                })
            };
            
            var header = new PlyHeader(PlyFormat.Ascii, elements);
            using (var stream = new MemoryStream())
            {
                var writer = new PlyAsciiWriter(stream, header);
                Assert.Throws<PlyWriteDataTypeMismatchException>(() => writer.WriteValue(12)); 
                writer.ForceDispose();
            }
        }

        [Test]
        public void ThrowsWhenNotAllValuesWritten()
        {
            var elements = new List<PlyElement>()
            {
                new PlyElement("vertex", 1, new List<PlyProperty>()
                {
                    new PlyProperty("x", PlyType.Float),
                }),
                new PlyElement("face", 1, new List<PlyProperty>()
                {
                    new PlyArrayProperty("vertex_index", PlyType.Int, PlyType.Int)
                })
            };
            
            var header = new PlyHeader(PlyFormat.Ascii, elements);
            using (var stream = new MemoryStream())
            {
                var writer = new PlyAsciiWriter(stream, header);
                writer.WriteValue(1.0f);
                Assert.Throws<PlyIterationNotFinishedException>(() => writer.Dispose());
                writer.ForceDispose();
            }
        }

        [Test]
        public void WritesCorrectlyAllDataTypesAsValues()
        {
            var elements = new List<PlyElement>() 
            { 
                new PlyElement("vertex", 2, new List<PlyProperty>()
                {  
                    new PlyProperty("p1", PlyType.Char),
                    new PlyProperty("p2", PlyType.Int),
                    new PlyProperty("p3", PlyType.Short),
                    new PlyProperty("p4", PlyType.Uchar),
                    new PlyProperty("p5", PlyType.Uint),
                    new PlyProperty("p6", PlyType.Ushort),
                    new PlyProperty("p7", PlyType.Float),
                    new PlyProperty("p8", PlyType.Double),
                }),
                new PlyElement("vertex2", 2, new List<PlyProperty>()
                {
                    new PlyProperty("float", PlyType.Float),
                    new PlyProperty("double", PlyType.Double)
                })
            };
            var header = new PlyHeader(PlyFormat.Ascii, elements);

            var actual = GetWriterOutput(header, (PlyWriter writer) => {
                writer.WriteValue(sbyte.MinValue);
                writer.WriteValue(int.MinValue);
                writer.WriteValue(short.MinValue);
                writer.WriteValue(byte.MinValue);
                writer.WriteValue(uint.MinValue);
                writer.WriteValue(ushort.MinValue);
                writer.WriteValue(float.MinValue);
                writer.WriteValue(double.MinValue);

                writer.WriteValue(sbyte.MaxValue);
                writer.WriteValue(int.MaxValue);
                writer.WriteValue(short.MaxValue);
                writer.WriteValue(byte.MaxValue);
                writer.WriteValue(uint.MaxValue);
                writer.WriteValue(ushort.MaxValue);
                writer.WriteValue(float.MaxValue);
                writer.WriteValue(double.MaxValue);

                writer.WriteValue(-123.00151f);
                writer.WriteValue(10000.04171d);
                writer.WriteValue(-100.0001f);
                writer.WriteValue(-100.00001d);
            });
            var expected = @"ply
format ascii 1.0
element vertex 2
property char p1
property int p2
property short p3
property uchar p4
property uint p5
property ushort p6
property float p7
property double p8
element vertex2 2
property float float
property double double
end_header
-128 -2147483648 -32768 0 0 0 -340282300000000000000000000000000000000 -179769313486232000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
127 2147483647 32767 255 4294967295 65535 340282300000000000000000000000000000000 179769313486232000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
-123.0015 10000.04171
-100.0001 -100.00001";
            Assert.AreEqual(actual, expected);
        }

        [Test]
        public void WritesCorrectlyAllDataTypesAsArrayIndexes()
        {
            var elements = new List<PlyElement>()
            {
                new PlyElement("face", 2, new List<PlyProperty>()
                {
                    new PlyProperty("p1", PlyType.Char),
                    new PlyProperty("p2", PlyType.Int),
                    new PlyProperty("p3", PlyType.Short),
                    new PlyProperty("p4", PlyType.Uchar),
                    new PlyProperty("p5", PlyType.Uint),
                    new PlyProperty("p6", PlyType.Ushort),
                    new PlyProperty("p7", PlyType.Float),
                    new PlyProperty("p8", PlyType.Double),
                }),
            };
            var header = new PlyHeader(PlyFormat.Ascii, elements);
        }

        [Test]
        public void WritesCorrectlyAllDataTypesAsArrayElementValues()
        {
            var elements = new List<PlyElement>()
            {
                new PlyElement("objects", 1, new List<PlyProperty>()
                {
                    new PlyArrayProperty("p1", PlyType.Int, PlyType.Char),
                    new PlyArrayProperty("p2", PlyType.Int, PlyType.Int),
                    new PlyArrayProperty("p3", PlyType.Int, PlyType.Short),
                    new PlyArrayProperty("p4", PlyType.Int, PlyType.Uchar),
                    new PlyArrayProperty("p5", PlyType.Int, PlyType.Uint),
                    new PlyArrayProperty("p6", PlyType.Int, PlyType.Ushort),
                    new PlyArrayProperty("p7", PlyType.Int, PlyType.Float),
                    new PlyArrayProperty("p8", PlyType.Int, PlyType.Double),
                })
            };

            var header = new PlyHeader(PlyFormat.Ascii, elements);
            var actual = GetWriterOutput(header, (PlyWriter writer) => {
                writer.WriteArray(sbyte.MinValue, sbyte.MaxValue);
                writer.WriteArray(int.MinValue, int.MaxValue);
                writer.WriteArray(short.MinValue, short.MaxValue);
                writer.WriteArray(byte.MinValue, byte.MaxValue);
                writer.WriteArray(uint.MinValue, uint.MaxValue);
                writer.WriteArray(ushort.MinValue, ushort.MaxValue);
                writer.WriteArray(float.MinValue, float.MaxValue);
                writer.WriteArray(double.MinValue, double.MaxValue);
            });

            var expected = @"ply
format ascii 1.0
element objects 1
property list int char p1
property list int int p2
property list int short p3
property list int uchar p4
property list int uint p5
property list int ushort p6
property list int float p7
property list int double p8
end_header
2 -128 127 2 -2147483648 2147483647 2 -32768 32767 2 0 255 2 0 4294967295 2 0 65535 2 -340282300000000000000000000000000000000 340282300000000000000000000000000000000 2 -179769313486232000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000 179769313486232000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
            Assert.AreEqual(expected, actual);
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

            var simpleHeader = new PlyHeader(PlyFormat.Ascii, simpleElements);

            var actual = GetWriterOutput(simpleHeader, (PlyWriter writer) => {
                writer.WriteValue(3.0f);
                writer.WriteValue(5.1113f);
                writer.WriteValue(-6.14f);

                writer.WriteValues(-0.1f, -0.000001f, 0f);

                writer.WriteArray(new int[] { 1, 2, 3 });
                writer.WriteArray(new int[] { -1, 5, 9 });
            });

            var expected = @"ply
format ascii 1.0
element vertex 2
property float x
property float y
property float z
element face 2
property list int int vertex_index
end_header
3 5.1113 -6.14
-0.1 -0.000001 0
3 1 2 3
3 -1 5 9";
            Assert.AreEqual(expected, actual);
        }

        private string GetWriterOutput(PlyHeader header, Action<PlyWriter> populateWriterFunc)
        {
            using (var stream = new MemoryStream())
            {
                using(var writer = new PlyAsciiWriter(stream, header))
                {
                    populateWriterFunc(writer);
                }
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    var actual = reader.ReadToEnd();
                    return actual;
                }
            }
        }
    }
}