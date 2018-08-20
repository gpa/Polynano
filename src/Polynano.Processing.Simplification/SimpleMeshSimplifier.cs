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
using Polynano.Processing.Core;
using Polynano.Processing.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Polynano.Processing.Simplification
{
    public class SimpleMeshSimplifier
    {
        private FastCandidateContainer<float> _contractionCandidates;
        private SimpleMesh _simpleMesh;
        protected float[] _vertexErrors;

        Stack<Tuple<IndexedEdge, OperationSnapshot>> _operationSnapshots;

        public SimpleMeshSimplifier(SimpleMesh simpleMesh)
        {
            _simpleMesh = simpleMesh;
            _contractionCandidates = new FastCandidateContainer<float>(simpleMesh.Vertices.Count);
            _vertexErrors = new float[simpleMesh.Vertices.Count];
            _operationSnapshots = new Stack<Tuple<IndexedEdge, OperationSnapshot>>();
        }

        public void Initialize()
        {
            foreach (var face in _simpleMesh.Faces)
            {
                _contractionCandidates.Add(face.Value.Edge1);
                _contractionCandidates.Add(face.Value.Edge2);
                _contractionCandidates.Add(face.Value.Edge3);
            }

            _simpleMesh.CleanVertices();

            foreach (var edge in _contractionCandidates.Edges)
                UpdateContractionCost(edge);
        }

        public bool SimplifyOneStep()
        {
            var candidate = _contractionCandidates.GetBestCandidate();
            if (candidate == null)
                return false;

            var v1 = candidate.Edge.Vertex1;
            var v2 = candidate.Edge.Vertex2;

            var operationSnapshot = _simpleMesh.ContractEdge(candidate.Edge, candidate.OptimalPosition);

            foreach (var face in operationSnapshot.Faces)
            {
                _contractionCandidates.Remove(face.Value.Edge1);
                _contractionCandidates.Remove(face.Value.Edge2);
                _contractionCandidates.Remove(face.Value.Edge3);

                if (_simpleMesh.Faces.ContainsKey(face.Key))
                {
                    var currentFace = _simpleMesh.Faces[face.Key];
                    _contractionCandidates.Add(currentFace.Edge1);
                    _contractionCandidates.Add(currentFace.Edge2);
                    _contractionCandidates.Add(currentFace.Edge3);
                }
            }

            _vertexErrors[candidate.Edge.Vertex1.Index] = candidate.Cost;

            foreach (var edge in _simpleMesh.GetVertexEdges(v1))
            {
                var otherVertex = edge.Vertex1 == v2 ? edge.Vertex2 : edge.Vertex1;
                foreach (var nv in _simpleMesh.GetVertexEdges(otherVertex))
                    UpdateContractionCost(nv);
            }

            _operationSnapshots.Push(Tuple.Create(candidate.Edge, operationSnapshot));
            return true;
        }

        public bool RevertOneStep()
        {
            if (_operationSnapshots.Count == 0)
                return false;

            var operationSnapshot = _operationSnapshots.Pop();
            foreach (var f in operationSnapshot.Item2.Faces)
            {
                if (_simpleMesh.Faces.ContainsKey(f.Key))
                {
                    var face = _simpleMesh.Faces[f.Key];
                    _contractionCandidates.Remove(face.Edge1);
                    _contractionCandidates.Remove(face.Edge2);
                    _contractionCandidates.Remove(face.Edge3);
                }
            }

            foreach (var f in operationSnapshot.Item2.Faces)
            {
                _contractionCandidates.Add(f.Value.Edge1);
                _contractionCandidates.Add(f.Value.Edge2);
                _contractionCandidates.Add(f.Value.Edge3);
            }

            _simpleMesh.RevertChanges(operationSnapshot.Item2);
      
            foreach (var edge in _simpleMesh.GetVertexEdges(operationSnapshot.Item1.Vertex1))
            {
                var otherVertex = edge.Vertex1 == operationSnapshot.Item1.Vertex2 ? edge.Vertex2 : edge.Vertex1;
                foreach (var nv in _simpleMesh.GetVertexEdges(otherVertex))
                    UpdateContractionCost(nv);
            }

            return true;
        }

        public bool HasSnapshots()
        {
            return _operationSnapshots.Count != 0;
        }

        private void UpdateContractionCost(IndexedEdge edge)
        {
            var v1 = _simpleMesh.Vertices[edge.Vertex1];
            var v2 = _simpleMesh.Vertices[edge.Vertex2];

            var opt = (v1.Position + v2.Position) / 2.0f;

            var candidate = new ContractionCandidate<float>(edge);

            var eo = _contractionCandidates.TryGetEdge(edge);

            float cost = 0;
            foreach (var faceRef in v1.ConnectedFaces.Concat(v2.ConnectedFaces))
            {
                var f = _simpleMesh.Faces[faceRef];
                var a = _simpleMesh.Vertices[f.Vertex1].Position;
                var b = _simpleMesh.Vertices[f.Vertex2].Position;
                var c = _simpleMesh.Vertices[f.Vertex3].Position;

                var v = b - a;
                var u = c - a;
                var n = Vector3.Cross(v, u);
                float dist = Vector3.Dot(opt - a, n);
                cost += Math.Abs(dist);
            }
           
            candidate.Cost = _vertexErrors[edge.Vertex1.Index] + _vertexErrors[edge.Vertex2.Index] + cost;
            candidate.OptimalPosition = opt;

            if (eo == null || eo.OptimalPosition != opt || eo.Cost != candidate.Cost)
            {
                _contractionCandidates.UpdateCandidate(candidate);
            }
        }
    }
}
