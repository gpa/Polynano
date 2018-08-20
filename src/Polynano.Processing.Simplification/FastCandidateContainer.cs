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

namespace Polynano.Processing.Simplification
{
    internal class FastCandidateContainer<TContractionError>
    {
        private readonly List<ContractionCandidate<TContractionError>>[] _edges;
        private readonly SortedDictionary<TContractionError, List<IndexedEdge>> _index;

        public FastCandidateContainer(int vertexCount)
        {
            _edges = new List<ContractionCandidate<TContractionError>>[vertexCount];
            _index = new SortedDictionary<TContractionError, List<IndexedEdge>>();
            for (int i = 0; i < vertexCount; i++)
                _edges[i] = new List<ContractionCandidate<TContractionError>>(2);
        }

        public IEnumerable<IndexedEdge> Edges
        {
            get
            {
                for (int i = 0; i < _edges.Length; i++)
                    foreach (var e in _edges[i])
                        yield return e.Edge;
            }
        }

        public bool Add(IndexedEdge edge)
        {
            if (_edges[edge.Vertex1.Index].Count(c => c.Edge.Vertex2 == edge.Vertex2) == 0)
            {
                _edges[edge.Vertex1.Index].Add(new ContractionCandidate<TContractionError>(edge));
                return true;
            }

            return false;
        }

        public bool Remove(IndexedEdge edge)
        {
            var e = TryGetEdge(edge);

            if (e == null)
                return false;

            RemoveFromIndex(e);
            _edges[edge.Vertex1.Index].RemoveAll(c => c.Edge.Vertex2 == edge.Vertex2);
            return true;
        }

        public bool Contains(IndexedEdge edge)
        {
            return _edges[edge.Vertex1.Index].Count(e => e.Edge.Vertex1 == edge.Vertex1 && e.Edge.Vertex2 == edge.Vertex2) > 0;
        }

        public void UpdateCandidate(ContractionCandidate<TContractionError> candidate)
        {
            var edge = GetCandidate(candidate.Edge);

            RemoveFromIndex(edge);
            edge.Cost = candidate.Cost;
            edge.OptimalPosition = candidate.OptimalPosition;

            if (_index.TryGetValue(edge.Cost, out var edges))
                edges.Add(edge.Edge);
            else
                _index.Add(edge.Cost, new List<IndexedEdge> { edge.Edge });
        }

        public ContractionCandidate<TContractionError> GetBestCandidate()
        {
            if (_index.Count == 0)
                return null;

            return GetCandidate(_index.First().Value.First());
        }

        public ContractionCandidate<TContractionError> TryGetEdge(IndexedEdge edge)
        {
            for (int i = 0; i < _edges[edge.Vertex1.Index].Count; i++)
                if (_edges[edge.Vertex1.Index][i].Edge.Vertex2 == edge.Vertex2)
                    return _edges[edge.Vertex1.Index][i];

            return null;
        }

        public ContractionCandidate<TContractionError> GetCandidate(IndexedEdge edge)
        {
            var e = TryGetEdge(edge);
            if (e == null)
                throw new ArgumentException(nameof(edge));

            return e;
        }

        private void RemoveFromIndex(ContractionCandidate<TContractionError> candidate)
        {
            var v1 = candidate.Edge.Vertex1;
            var v2 = candidate.Edge.Vertex2;
            if (_index.TryGetValue(candidate.Cost, out var edges))
            {
                for (int i = 0; i < edges.Count; i++)
                {
                    if (edges[i].Vertex1 == v1 && edges[i].Vertex2 == v2)
                    {
                        var temp = edges[edges.Count - 1];
                        edges[edges.Count - 1] = edges[i];
                        edges[i] = temp;
                        edges.RemoveAt(edges.Count - 1);
                        break;
                    }
                }

                if (edges.Count == 0)
                    _index.Remove(candidate.Cost);
            }
        }
    }
}
