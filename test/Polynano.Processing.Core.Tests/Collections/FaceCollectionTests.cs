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
using Polynano.Processing.Core.Collections;
using System.Linq;

namespace Polynano.Processing.Core.Tests.Collections
{
    [TestFixture]
    class FaceCollectionTests
    {

        [Test]
        public void CountAndAccessorTest()
        {
            var faceCollection = new FaceCollection(2, 3);
            Assert.AreEqual(0, faceCollection.Count);
        }

        [Test]
        public void AddFace_AddedFaceCanBeAccessed()
        {
            var faceCollection = new FaceCollection(2, 3);
            faceCollection.AddFace(1, 2, 3);
            Assert.AreEqual(1, faceCollection.Count);
            var f = faceCollection.First();
            Assert.AreEqual(3, f.Count);
            Assert.AreEqual(1, f[0]);
            Assert.AreEqual(2, f[1]);
            Assert.AreEqual(3, f[2]);
        }

        [Test]
        public void AddFace_MultipleAddedFacesCanBeAccessed()
        {
            var faceCollection = new FaceCollection(2, 3);
            faceCollection.AddFace(1, 2, 3);
            faceCollection.AddFace(3, 2, 1, 4, 5);
            Assert.AreEqual(2, faceCollection.Count);
            var f = faceCollection.First();
            Assert.AreEqual(3, f.Count);
            Assert.AreEqual(1, f[0]);
            Assert.AreEqual(2, f[1]);
            Assert.AreEqual(3, f[2]);
            var f2 = faceCollection.Skip(1).First();
            Assert.AreEqual(5, f2.Count);
            Assert.AreEqual(4, f2[3]);
        }
    }
}
