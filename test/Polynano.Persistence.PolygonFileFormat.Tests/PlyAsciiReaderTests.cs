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
using Polynano.Persistence.PolygonFileFormat.Extensions;
using Polynano.Persistence.PolygonFileFormat.Readers;
using Polynano.Persistence.PolygonFileFormat.Readers.StreamReaders;
using System.IO;
using System.Linq;
using System.Text;

namespace Polynano.Persistence.Tests
{

    [TestFixture]
    public class PlyAsciiReaderTests
    {
        [Test]
        public void Parse_ParsesBasicDataCorrectly()
        {
            var content = @"ply
format ascii 1.0
element vertex 3
property float32 x
property float32 y
property float32 z
element face 4
property list uint8 int32 vertex_indices
end_header
1.13927 0.985002 0.534429 
1.11738 0.998603 0.513986 
1.17148 1.02159 0.494923  
3 0 1 2 
3 0 2 3 
3 4 5 1 
6 4 1 0 1 2 3";

            using (var stream = GetStream(content))
            {
                var streamReader = new BufferedStreamReader(stream);
                var headerReader = new PlyHeaderReader(streamReader);
                var header = headerReader.Read();
                var reader = header.GetDataReader(streamReader);

                Assert.AreEqual(1.13927f, reader.ReadProperty<float>());
                Assert.AreEqual(0.985002f, reader.ReadProperty<float>());
                Assert.AreEqual(0.534429f, reader.ReadProperty<float>());
                Assert.AreEqual(1.11738f, reader.ReadProperty<float>());
                reader.SkipProperty(5);
                CollectionAssert.AreEqual(new int[] { 0, 1, 2 }, reader.ReadArray<int>().ToList());
                reader.SkipProperty(2);
                CollectionAssert.AreEqual(new int[] { 4, 1, 0, 1, 2, 3 }, reader.ReadArray<int>().ToList());
            }
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