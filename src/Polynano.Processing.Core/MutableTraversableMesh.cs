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
using Polynano.Processing.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Polynano.Processing.Core
{
    public class MutableTraversableMesh : TraversableMesh
    {
        private readonly List<Tuple<HalfedgeRef, HalfedgeRef>> _updateBuffer;

        public MutableTraversableMesh(MeshData meshData, NonManifoldBehavior nonManifoldBehavior)
         : base(meshData, nonManifoldBehavior)
        {
            _updateBuffer = new List<Tuple<HalfedgeRef, HalfedgeRef>>(5);
        }

        public MutableTraversableMesh(MeshData mesh)
            : base(mesh)
        {
        }

        public void RemoveFace(FaceRef faceRef)
        {
            if (!_faces.ContainsKey(faceRef))
                throw new ArgumentException($"Trying to remove invalid face ${faceRef.Index}");

            var faceHalfedgeRefs = EnumerateHalfedges(faceRef).ToList();
            foreach (var halfedgeRef in faceHalfedgeRefs)
            {
                var currentRef = halfedgeRef;
                var current = GetHalfedge(halfedgeRef);
                var oppositeRef = GetOpposite(halfedgeRef);
                var opposite = GetHalfedge(oppositeRef);

                _halfedges[currentRef] = new Halfedge(current, FaceRef.None);
                if (opposite.Face.IsNone())
                {
                    _halfedges.Remove(currentRef);
                    _halfedges.Remove(oppositeRef);

                    if (_vertices[current.Vertex].MemberHalfedge == currentRef)
                        UpdateVertex(current.Vertex);

                    if (_vertices[opposite.Vertex].MemberHalfedge == oppositeRef)
                        UpdateVertex(opposite.Vertex);
                }
            }

            foreach (var halfedgeRef in faceHalfedgeRefs)
            {
                var currentRef = halfedgeRef;
                var oppositeRef = GetOpposite(halfedgeRef);
                if (!_halfedges.ContainsKey(currentRef))
                {
                    var previousOfOppositeRef = GetPrevious(oppositeRef);
                    var previousOfOpposite = GetHalfedge(previousOfOppositeRef);
                    var nextOfOpposite = GetNext(oppositeRef);
                    var previousOfCurrentRef = GetPrevious(currentRef);
                    var previousOfCurrent = GetHalfedge(previousOfCurrentRef);
                    var nextOfCurrentRef = GetNext(currentRef);

                    if (!_halfedges.ContainsKey(nextOfCurrentRef))
                        _updateBuffer.Add(Tuple.Create(previousOfOppositeRef, GetNext(GetOpposite(nextOfCurrentRef))));
                    else
                        _updateBuffer.Add(Tuple.Create(previousOfOppositeRef, nextOfCurrentRef));

                    _updateBuffer.Add(Tuple.Create(previousOfCurrentRef, nextOfOpposite));
                }
            }

            BufferPerformUpdates();
            _faces.Remove(faceRef);

            SanityCheck();
        }

        public void RemoveVertex(VertexRef vertexRef)
        {
            var faces = EnumerateFaces(vertexRef).ToArray();
            foreach (var face in faces)
                RemoveFace(face);

            SanityCheck();
        }

        public void ContractEdge(IndexedEdge edge, Vector3 newPosition)
        {
            var halfedgeRef = GetHalfedgeForEdge(edge);
            var halfedge = GetHalfedge(halfedgeRef);
            var halfedgeFace = halfedge.Face;

            var oppositeRef = GetOpposite(halfedgeRef);
            var opposite = GetHalfedge(oppositeRef);
            var oppositeFace = opposite.Face;

            HalfedgeRef[] remove = new []{ HalfedgeRef.None, HalfedgeRef.None };
            HalfedgeRef[] removePrevious = new[]  { HalfedgeRef.None, HalfedgeRef.None };

            if (!halfedgeFace.IsNone())
            {
                remove[0] = GetPrevious(halfedgeRef);
                removePrevious[0] = GetPrevious(remove[0]);
            }

            if (!oppositeFace.IsNone())
            {
                remove[1] = GetPrevious(GetOpposite(halfedgeRef));
                removePrevious[1] = GetPrevious(remove[1]);
            }

            if (!halfedgeFace.IsNone())
                RemoveFace(halfedge.Face);

            if (!oppositeFace.IsNone())
                RemoveFace(opposite.Face);

            for (int i = 0; i < remove.Length; ++i)
            {
                if (remove[i].IsNone() || !_halfedges.ContainsKey(remove[i]) || (!removePrevious[i].IsNone() && !_halfedges.ContainsKey(removePrevious[i])))
                    continue;

                var currentRef = remove[i];
                var currentOppositeRef = GetOpposite(currentRef);
                var previousRef = GetPrevious(currentRef);
                var nextOfOppositeRef = GetNext(currentOppositeRef);
                var previousOfOppositeRef = GetPrevious(currentOppositeRef);

                if (GetHalfedge(currentOppositeRef).Face.IsNone())
                    continue;

                var faceRef = GetHalfedge(currentOppositeRef).Face;
                if (_faces[faceRef].MemberHalfedge == currentOppositeRef)
                    _faces[faceRef] = new HalfedgeFace(_faces[faceRef], GetNext(currentOppositeRef));

                _halfedges[previousRef] = new Halfedge(_halfedges[previousRef], GetHalfedge(currentOppositeRef).Face);
                _updateBuffer.Add(Tuple.Create(previousRef, nextOfOppositeRef));
                _updateBuffer.Add(Tuple.Create(previousOfOppositeRef, previousRef));

                var v1 = GetHalfedge(currentRef).Vertex;
                var v2 = GetHalfedge(currentOppositeRef).Vertex;

                _halfedges.Remove(currentRef);
                _halfedges.Remove(currentOppositeRef);

                UpdateVertex(v1);
                UpdateVertex(v2);
            }
           
            var v2Halfedges = EnumerateOneRing(edge.Vertex2).ToList();
            BufferPerformUpdates();

            foreach (var v2HalfedgeRef in v2Halfedges)
            {
                if (!_halfedges.ContainsKey(v2HalfedgeRef))
                    continue;

                if (GetHalfedge(v2HalfedgeRef).Vertex == edge.Vertex2)
                    _halfedges[v2HalfedgeRef] = new Halfedge(_halfedges[v2HalfedgeRef], edge.Vertex1);
            }

            if (_vertices.ContainsKey(edge.Vertex2))
                _vertices.Remove(edge.Vertex2);

            _vertices[edge.Vertex1] = new HalfedgeVertex(_vertices[edge.Vertex1], newPosition);

            SanityCheck();
        }

        private void UpdateVertex(VertexRef vertexRef)
        {
            var currentRef = _vertices[vertexRef].MemberHalfedge;

            if (!currentRef.IsNone() && _halfedges.ContainsKey(currentRef))
                return;

            HalfedgeRef candidateHalfedgeRef = currentRef;
            do
            {
                candidateHalfedgeRef = GetPrevious(GetOpposite(candidateHalfedgeRef));
                if (!candidateHalfedgeRef.IsNone() && _halfedges.ContainsKey(candidateHalfedgeRef))
                {
                    _vertices[vertexRef] = new HalfedgeVertex(_vertices[vertexRef], candidateHalfedgeRef);
                    break;
                }
            } while (candidateHalfedgeRef != currentRef);

            if (candidateHalfedgeRef == currentRef)
            {
                _vertices.Remove(vertexRef);
            }
        }

        private void BufferPerformUpdates()
        {
            foreach (var update in _updateBuffer)
                _halfedges[update.Item1] = new Halfedge(_halfedges[update.Item1], update.Item2);
            _updateBuffer.Clear();
        }

        private void SanityCheck()
        {
#if DEBUG_SANITY_CHECK
            foreach(var face in _faces)
            {
                if (!_halfedges.ContainsKey(face.Value.MemberHalfedge))
                    throw new InvalidOperationException();

                EnumerateNeighbours(face.Key);
                EnumerateVertices(face.Key);
                EnumerateHalfedges(face.Key);
            }

            foreach(var vertex in _vertices)
            {
                if (!_halfedges.ContainsKey(vertex.Value.MemberHalfedge))
                    throw new InvalidOperationException();

                EnumerateOneRing(vertex.Key);
            }

            foreach(var halfedge in _halfedges)
            {
                if (!_halfedges.ContainsKey(halfedge.Value.Next))
                    throw new InvalidOperationException();

                if (!_vertices.ContainsKey(halfedge.Value.Vertex))
                    throw new InvalidOperationException();
            
                if (!halfedge.Value.Face.IsNone() && !_faces.ContainsKey(halfedge.Value.Face))
                    throw new InvalidOperationException();
            }
#endif
        }
    }
}