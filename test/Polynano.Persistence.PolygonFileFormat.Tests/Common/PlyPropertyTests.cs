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

namespace Polynano.Persistence.Tests
{
    [TestFixture]
    public class PlyPropertyTests
    {
        private const PlyType InvalidDataType = (PlyType)123;

        [Test]
        public void Constructor_ThrowsWhenNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new PlyProperty(null, PlyType.Float));
        }

        [Test]
        public void Constructor_ThrowsWhenNameIsNotAToken()
        {
            Assert.Throws<ArgumentException>(() => new PlyProperty(" 53 fwf a54", PlyType.Float));
        }

        [Test]
        public void Constructor_ThrowsWhenDataTypeIsInvalid()
        {
            Assert.Throws<ArgumentException>(() => new PlyProperty("x", InvalidDataType));
        }
    }
}