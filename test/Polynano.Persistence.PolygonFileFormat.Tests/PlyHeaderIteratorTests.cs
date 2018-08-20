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
using System.Collections.Generic;

namespace Polynano.Persistence.Tests
{
    [TestFixture]
    public class PlyHeaderIteratorTests
    {
        public PlyHeader ValidHeader { get; private set; }

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

            ValidHeader = new PlyHeader(PlyFormat.Ascii, "Comment", null, elements);
        }

        [TearDown]
        public void Teardown()
        {
            ValidHeader = null;
        }

        [Test]
        public void KeepsTrackOfElements()
        {
            var iterator = new PlyHeaderIterator(ValidHeader);
            Assert.IsTrue(iterator.IsOnFirstElement);
            Assert.IsTrue(iterator.IsOnFirstProperty);
            Assert.IsFalse(iterator.IsIterationDone);
            Assert.AreEqual(iterator.CurrentProperty, ValidHeader.Elements[0].Properties[0]);

            iterator.MoveNext();
            Assert.IsFalse(iterator.IsOnFirstElement);
            Assert.IsFalse(iterator.IsOnFirstProperty);
            Assert.IsFalse(iterator.IsIterationDone);
            Assert.AreEqual(iterator.CurrentElement, 0);
            Assert.AreEqual(iterator.CurrentProperty, ValidHeader.Elements[0].Properties[1]);

            iterator.MoveNext();
            iterator.MoveNext();
            Assert.AreEqual(iterator.CurrentElement, 0);
            Assert.AreEqual(iterator.CurrentProperty, ValidHeader.Elements[0].Properties[0]);

            iterator.MoveNext();
            iterator.MoveNext();
            iterator.MoveNext();
            Assert.IsFalse(iterator.IsOnFirstElement);
            Assert.AreEqual(iterator.CurrentProperty, ValidHeader.Elements[1].Properties[0]);
            Assert.IsTrue(iterator.IsOnFirstProperty);
            iterator.MoveNext();
            iterator.MoveNext();
            Assert.IsTrue(iterator.IsIterationDone);
        }
    }
}