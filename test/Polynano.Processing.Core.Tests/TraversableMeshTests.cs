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
using Polynano.Processing.Core.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Polynano.Processing.Core.Tests
{
    [TestFixture]
    class TraversableMeshTests : TraversableMeshTestBase
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
        public void EnumerateVertices_CorrectlyEnumeratesSimpleTriangle()
        {
            AddDummyVertices(3);
            AddFace(2, 0, 1);
            var tm = GetTraversableMesh();

            Assert.AreEqual(1, tm.Faces.Count);
            var indices = tm.EnumerateVertices(tm.Faces.First().Key).ToList();
            Assert.AreEqual(0, indices[0].Index);
            Assert.AreEqual(1, indices[1].Index);
            Assert.AreEqual(2, indices[2].Index);
        }


        [Test]
        public void EdgesProperty_CorrectlyEnumeratesSimpleTriangle()
        {
            AddDummyVertices(3);
            AddFace(2, 0, 1);
            var tm = GetTraversableMesh();

            var edges = tm.Edges.ToList();
            Assert.AreEqual(0, edges[0].Vertex1.Index);
            Assert.AreEqual(2, edges[0].Vertex2.Index);
            Assert.AreEqual(0, edges[1].Vertex1.Index);
            Assert.AreEqual(1, edges[1].Vertex2.Index);
            Assert.AreEqual(1, edges[2].Vertex1.Index);
            Assert.AreEqual(2, edges[2].Vertex2.Index);
        }

        [Test]
        public void Constructor_SetsNextReferencesCorrectly()
        {
            AddDummyVertices(5);
            AddFace(0, 1, 2);
            var tm = GetTraversableMesh();
            var he0 = tm.Faces[new FaceRef(0)].MemberHalfedge;
            var he1 = tm.Halfedges[he0].Next;
            var he2 = tm.Halfedges[he1].Next;
            var he3 = tm.Halfedges[he2].Next;
            Assert.IsTrue(he0 == he3);
        }

        [Test]
        public void GetPrevious_ReturnsThePreviousHalfedge()
        {
            AddDummyVertices(5);
            AddFace(0, 1, 2);
            var tm = GetTraversableMesh();
            var he0 = tm.Faces[new FaceRef(0)].MemberHalfedge;
            var he1 = tm.GetPrevious(he0);
            var he2 = tm.GetPrevious(he1);
            var he3 = tm.GetPrevious(he2);
            Assert.IsTrue(he0 == he3);
        }

        [Test]
        public void Constructor_CorrectlyInitializesOutlineHalfedges()
        {
            AddDummyVertices(3);
            AddFace(2, 0, 1);
            var tm = GetTraversableMesh();
            var he0 = tm.GetOpposite(tm.Faces[new FaceRef(0)].MemberHalfedge);
            var he1 = tm.Halfedges[he0].Next;
            var he2 = tm.Halfedges[he1].Next;
            var he3 = tm.Halfedges[he2].Next;

            Assert.IsFalse(he0 == tm.Faces[new FaceRef(0)].MemberHalfedge);
            Assert.IsTrue(he3 == he0);
        }

        [Test]
        public void Constructor_CorrectlyInitializesSimpleStructure()
        {
            AddDummyVertices(7);
            AddFace(2, 1, 0);
            AddFace(3, 2, 0);
            AddFace(4, 3, 0);
            AddFace(5, 4, 0);
            AddFace(6, 5, 0);
            AddFace(6, 0, 1);
            var tm = GetTraversableMesh();
            var he = tm.Vertices[new VertexRef(0)].MemberHalfedge;
            var outlineHe = tm.GetOpposite(tm.GetNext(tm.GetNext(he)));
            var startedAt = outlineHe;
            outlineHe = tm.GetNext(outlineHe);
            Assert.AreNotEqual(outlineHe, startedAt);
            outlineHe = tm.GetNext(outlineHe);
            Assert.AreNotEqual(outlineHe, startedAt);
            outlineHe = tm.GetNext(outlineHe);
            Assert.AreNotEqual(outlineHe, startedAt);
            outlineHe = tm.GetNext(outlineHe);
            Assert.AreNotEqual(outlineHe, startedAt);
            outlineHe = tm.GetNext(outlineHe);
            Assert.AreNotEqual(outlineHe, startedAt);
            outlineHe = tm.GetNext(outlineHe);
            Assert.AreEqual(outlineHe, startedAt);
        }

        [Test]
        public void EnumerateNeighbours_CorrectlyReturnsAllFaceNeighbours()
        {
            AddDummyVertices(7);
            AddFace(2, 1, 0);
            AddFace(3, 2, 0);
            AddFace(4, 3, 0);
            AddFace(5, 4, 0);
            AddFace(6, 5, 0);
            AddFace(6, 0, 1);
            var tm = GetTraversableMesh();
            var faceRef = new FaceRef(0);
            var neighbours = tm.EnumerateNeighbours(faceRef).Select(x => x.Index).ToArray();
            CollectionAssert.AreEquivalent(new int[] { 1, 2, 3, 4, 5 }, neighbours);
        }

        [Test]
        public void EnumerateOneRing_CorrectlyEnumeratesAVertex()
        {
            AddDummyVertices(7);
            AddFace(2, 1, 0);
            AddFace(3, 2, 0);
            AddFace(4, 3, 0);
            AddFace(5, 4, 0);
            AddFace(6, 5, 0);
            AddFace(6, 0, 1);
            var tm = GetTraversableMesh();
            var halfedges = tm.EnumerateOneRing(new VertexRef(0)).ToList();
            CollectionAssert.AreEqual(new int[] { 0, 0, 0, 0, 0, 0 }, halfedges.Select(x => tm.Halfedges[x].Vertex.Index).ToArray());
        }
    }
}
