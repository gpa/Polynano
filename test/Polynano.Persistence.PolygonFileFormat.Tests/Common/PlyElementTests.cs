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
using System;
using System.Collections.Generic;

namespace Polynano.Persistence.Tests
{
    [TestFixture]
    public class PlyElementTests
    {
        private const PlyFormat InvalidFormat = (PlyFormat)123;

        private IList<PlyProperty> ValidPropertyList = new List<PlyProperty>() { new PlyProperty("x", PlyType.Float )};

        [Test]
        public void Constructor_ThrowsWhenNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new PlyElement(null, 1, ValidPropertyList));
        }

        [Test]
        public void Constructor_ThrowsWhenNameIsNotAToken()
        {
            Assert.Throws<ArgumentException>(() => new PlyElement(" dwa wa", 1, ValidPropertyList));
        }
        
        [Test]
        public void Constructor_ThrowsWhenInstanceCountIsInvalid()
        {
            Assert.Throws<ArgumentException>(() => new PlyElement("vertex", 0, ValidPropertyList));
            Assert.Throws<ArgumentException>(() => new PlyElement("vertex", -1, ValidPropertyList));
        }

        [Test]
        public void Constructor_ThrowsWhenPropertyListIsEmptyOrNull()
        {
            Assert.Throws<ArgumentException>(() => new PlyElement("vertex", 1, new List<PlyProperty>()));
            Assert.Throws<ArgumentNullException>(() => new PlyElement("vertex", 1, null));
        }
    }
}