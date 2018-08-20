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
using Polynano.Core.Collections;
using Polynano.Processing.Core.Collections;
using Polynano.Processing.Core.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Polynano.Processing.Core
{
    public class TraversableMesh
    {
        protected readonly SoftDeleteList<HalfedgeRef, Halfedge> _halfedges;
        protected readonly SoftDeleteList<FaceRef, HalfedgeFace> _faces;
        protected readonly SoftDeleteList<VertexRef, HalfedgeVertex> _vertices;

        public IReadOnlyDictionary<HalfedgeRef, Halfedge> Halfedges => _halfedges;
        public IReadOnlyDictionary<FaceRef, HalfedgeFace> Faces => _faces;
        public IReadOnlyDictionary<VertexRef, HalfedgeVertex> Vertices => _vertices;
        public IReadOnlyCollection<IndexedEdge> Edges => new EdgeCollectionAdapter(_halfedges);

        public TraversableMesh(MeshData meshData)
            : this(meshData, NonManifoldBehavior.Throw)
        {
        }

        public TraversableMesh(MeshData meshData, NonManifoldBehavior nonManifoldBehavior)
        {
            _halfedges = new SoftDeleteList<HalfedgeRef, Halfedge>(meshData.Faces.Count * 2);
            _faces = new SoftDeleteList<FaceRef, HalfedgeFace>(meshData.Faces.Count);
            _vertices = new SoftDeleteList<VertexRef, HalfedgeVertex>(meshData.Vertices.Count);

            for (int i = 0; i < meshData.Vertices.Count; i++)
                _vertices.Add(new HalfedgeVertex(meshData.Vertices[i], HalfedgeRef.None));

            for (int i = 0; i < meshData.Faces.Count; ++i)
                _faces.Add(new HalfedgeFace(HalfedgeRef.None));

            InitializeHalfedgeStructure(meshData.Faces, nonManifoldBehavior);
        }

        public HalfedgeRef GetOpposite(HalfedgeRef halfedgeRef)
        {
            return new HalfedgeRef(halfedgeRef.Index ^ 1);
        }

        public HalfedgeRef GetNext(HalfedgeRef halfedgeRef)
        {
            return Halfedges[halfedgeRef].Next;
        }

        public HalfedgeRef GetPrevious(HalfedgeRef halfedgeRef)
        {
            HalfedgeRef resultRef = GetNext(halfedgeRef);

            if (resultRef == HalfedgeRef.None)
                return resultRef;

            Halfedge result = Halfedges[resultRef];
            while (result.Next != halfedgeRef)
            {
                resultRef = result.Next;
                result = Halfedges[resultRef];
            }
            return resultRef;
        }

        public Halfedge GetHalfedge(HalfedgeRef halfedgeRef)
        {
            return _halfedges[halfedgeRef];
        }

        public IEnumerable<HalfedgeRef> EnumerateHalfedges(FaceRef faceRef)
        {
            HalfedgeRef begin = Faces[faceRef].MemberHalfedge;
            HalfedgeRef o = begin;
            do
            {
                yield return o;
                o = GetNext(o);
            } while (begin != o);
        }

        public IEnumerable<HalfedgeRef> EnumerateOneRing(VertexRef vertexRef)
        {
            HalfedgeRef h = Vertices[vertexRef].MemberHalfedge;
            HalfedgeRef hstop = h;

            do
            {
                if (h.IsNone())
                    yield break;

                yield return h;
                h = GetOpposite(GetNext(h));
            } while (h != hstop);
        }

        public IEnumerable<VertexRef> EnumerateVertices(FaceRef faceRef)
        {
            return EnumerateHalfedges(faceRef).Select(he => Halfedges[he].Vertex);
        }

        public IEnumerable<FaceRef> EnumerateNeighbours(FaceRef faceRef)
        {
            foreach (var he in EnumerateHalfedges(faceRef))
            {
                HalfedgeRef current = GetNext(GetOpposite(he));
                FaceRef currentFace = Halfedges[he].Face;
                if (current.IsNone())
                    continue;
                do
                {
                    var opposite = GetOpposite(current);

                    if (GetNext(opposite).IsNone())
                        break;

                    current = GetNext(opposite);
                    if (Halfedges[current].Face != currentFace && !Halfedges[current].Face.IsNone())
                        yield return Halfedges[current].Face;

                } while (currentFace != Halfedges[current].Face);
            }
        }

        public IEnumerable<FaceRef> EnumerateFaces(VertexRef vertexRef)
        {
            foreach (var halfedgeRef in EnumerateOneRing(vertexRef))
            {
                if (_halfedges[halfedgeRef].Face.IsNone())
                    continue;

                yield return _halfedges[halfedgeRef].Face;
            }
        }

        public IEnumerable<FaceRef> EnumerateFaces(IndexedEdge indexedEdge)
        {
            foreach (var currentRef in EnumerateOneRing(indexedEdge.Vertex1))
            {
                var opposite = _halfedges[GetOpposite(currentRef)];
                if (opposite.Vertex == indexedEdge.Vertex2)
                {
                    if (!_halfedges[currentRef].Face.IsNone())
                        yield return _halfedges[currentRef].Face;
                    
                    if (!opposite.Face.IsNone())
                        yield return opposite.Face;

                    yield break;
                }
            }
        }

        public IndexedEdge GetEdgeForHalfedge(HalfedgeRef halfedgeRef)
        {
            return new IndexedEdge(_halfedges[halfedgeRef].Vertex, _halfedges[GetOpposite(halfedgeRef)].Vertex);
        }

        public HalfedgeRef GetHalfedgeForEdge(IndexedEdge indexedEdge)
        {
            foreach (var currentRef in EnumerateOneRing(indexedEdge.Vertex1))
                if (GetHalfedge(GetOpposite(currentRef)).Vertex == indexedEdge.Vertex2)
                    return currentRef;

            return HalfedgeRef.None;
        }

        private void InitializeHalfedgeStructure(IReadOnlyCollection<IFace> faces, NonManifoldBehavior nonManifoldBehavior)
        {
            Dictionary<long, HalfedgeRef> oppositeBuffer = new Dictionary<long, HalfedgeRef>();
            List<HalfedgeRef> nextBuffer = new List<HalfedgeRef>();

            // Walk through all faces
            FaceRef fi = FaceRef.None;
            foreach (var face in faces)
            {
                fi = new FaceRef(fi.Index + 1);
                _faces[fi] = new HalfedgeFace(HalfedgeRef.None);

                nextBuffer.Clear();

                // for each of the face edges
                int bi = _halfedges.Count;
                for (int i = 0; i < face.Count; i++)
                {
                    var a = new VertexRef(face[i]);
                    var b = new VertexRef(face[(i + 1) % face.Count]);

                    // find if there is already a opposite halfedge added
                    HalfedgeRef placeholderIndex = HalfedgeRef.None;
                    if (oppositeBuffer.TryGetValue(CombineBits(b.Index, a.Index), out placeholderIndex))
                    {
                        var he = _halfedges[placeholderIndex];
                        _halfedges[placeholderIndex] = new Halfedge(he.Vertex, fi, he.Next);
                        nextBuffer.Add(placeholderIndex);
                    }
                    else
                    {
                        // If not, add the halfedge and add a placeholder for the opposite halfedge
                        var current = new Halfedge(b, fi, HalfedgeRef.None);
                        var opposite = new Halfedge(a, FaceRef.None, HalfedgeRef.None);

                        _halfedges.Add(current);
                        _halfedges.Add(opposite);

                        var currentRef = new HalfedgeRef(_halfedges.Count - 2);
                        var oppositeRef = new HalfedgeRef(_halfedges.Count - 1);

                        if (_vertices[b].MemberHalfedge.IsNone())
                            _vertices[b] = new HalfedgeVertex(_vertices[b].Position, currentRef);
                        if (_vertices[a].MemberHalfedge.IsNone())
                            _vertices[a] = new HalfedgeVertex(_vertices[a].Position, oppositeRef);

                        // register for subsequent next matching
                        nextBuffer.Add(currentRef);

                        // Register this halfedge for future opposite halfedge search
                        oppositeBuffer.Add(CombineBits(a.Index, b.Index), oppositeRef);
                    }
                }

                // Bind a halfedge to the face
                _faces[fi] = new HalfedgeFace(nextBuffer[0]);

                // Update the next references
                nextBuffer.Add(nextBuffer[0]);
                for (int i = 1; i < nextBuffer.Count; i++)
                {
                    var pi = nextBuffer[i - 1];
                    var replacementHalfedge = nextBuffer[i];
                    _halfedges[pi] = new Halfedge(_halfedges[pi].Vertex, _halfedges[pi].Face, replacementHalfedge);
                }
            }

            // Find all halfedges that don't have nexts, meaning they are located on the mesh outline,
            // or on a hole.
            var outlineHalfedges = new Dictionary<VertexRef, HalfedgeRef>();
            foreach (var halfedge in _halfedges)
            {
                if (halfedge.Value.Next.IsNone())
                {
                    var oppositeVertex = Halfedges[GetOpposite(halfedge.Key)].Vertex;
                    outlineHalfedges.Add(oppositeVertex, halfedge.Key);
                }
            }

            // Update next halfedge references
            foreach (var pair in outlineHalfedges)
            {
                var withoutNext = _halfedges[pair.Value];
                var nextHalfedge = outlineHalfedges[withoutNext.Vertex];
                _halfedges[pair.Value] = new Halfedge(withoutNext.Vertex, withoutNext.Face, nextHalfedge);
            }
        }

        private long CombineBits(int i, int j)
        {
            return unchecked(((long)i << 32) | ((long)j));
        }
    }
}
