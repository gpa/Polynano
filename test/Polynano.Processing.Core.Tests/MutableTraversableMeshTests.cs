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
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Polynano.Processing.Core.Tests
{
    [TestFixture]
    class MutableTraversableMeshTests : TraversableMeshTestBase
    {
        [SetUp]
        public void SetUp()
        {
            Vertices = new List<Vector3>();
            Faces = new FaceCollection(30, 3);
        }

        [TearDown]
        public void TearDown()
        {
            Vertices = null;
            Faces = null;
        }

        [Test]
        public void RemoveFace_RemovesCorrectlyAllMeshElements()
        {
            AddDummyVertices(3);
            AddFace(2, 0, 1);
            var mtm = GetMutableTraversableMesh();

            Assert.AreEqual(1, mtm.Faces.Count);
            mtm.RemoveFace(mtm.Faces.First().Key);

            Assert.AreEqual(0, mtm.Faces.Count);
            Assert.AreEqual(0, mtm.Vertices.Count);
            Assert.AreEqual(0, mtm.Halfedges.Count);
            Assert.AreEqual(0, mtm.Edges.Count);
        }

        [Test]
        public void RemoveVertex_RemovesCorrectlyAllMeshElements()
        {
            AddDummyVertices(3);
            AddFace(2, 0, 1);
            var mtm = GetMutableTraversableMesh();

            Assert.AreEqual(3, mtm.Vertices.Count);
            mtm.RemoveVertex(mtm.Vertices.First().Key);

            Assert.AreEqual(0, mtm.Faces.Count);
            Assert.AreEqual(0, mtm.Vertices.Count);
            Assert.AreEqual(0, mtm.Halfedges.Count);
            Assert.AreEqual(0, mtm.Edges.Count);
        }

        [Test]
        public void CollapseEdge_RemovesCorrectlyAllMeshElements()
        {
            AddDummyVertices(3);
            AddFace(2, 0, 1);
            var mtm = GetMutableTraversableMesh();

            Assert.AreEqual(3, mtm.Edges.Count);
            mtm.ContractEdge(mtm.Edges.First(), Vector3.Zero);

            Assert.AreEqual(0, mtm.Faces.Count);
            Assert.AreEqual(0, mtm.Vertices.Count);
            Assert.AreEqual(0, mtm.Halfedges.Count);
            Assert.AreEqual(0, mtm.Edges.Count);
        }

        [Test]
        public void CollapseEdge_CorrectlyManipulatesHalfedges()
        {
            AddDummyVertices(3);
            AddFace(2, 0, 1);
            var mtm = GetMutableTraversableMesh();

            Assert.AreEqual(3, mtm.Edges.Count);
            mtm.ContractEdge(mtm.Edges.First(), Vector3.Zero);

            Assert.AreEqual(0, mtm.Faces.Count);
            Assert.AreEqual(0, mtm.Vertices.Count);
            Assert.AreEqual(0, mtm.Halfedges.Count);
            Assert.AreEqual(0, mtm.Edges.Count);
        }
    }
}
