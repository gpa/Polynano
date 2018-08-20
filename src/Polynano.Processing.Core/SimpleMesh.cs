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
using Polynano.Processing.Core.Collections;
using Polynano.Processing.Core.Geometry;
using Polynano.Processing.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Polynano.Processing.Core
{
    public class SimpleMesh
    {
        protected SoftDeleteList<VertexRef, VertexWithFaceBindings> _vertices;
        protected SoftDeleteList<FaceRef, IndexedTriangle> _faces;

        public IReadOnlyDictionary<VertexRef, VertexWithFaceBindings> Vertices 
            => _vertices;
        public IReadOnlyDictionary<FaceRef, IndexedTriangle> Faces 
            => _faces;
        
        public SimpleMesh(MeshData meshData)
        {
            _vertices = new SoftDeleteList<VertexRef, VertexWithFaceBindings>(meshData.Vertices.Count);
            _faces = new SoftDeleteList<FaceRef, IndexedTriangle>(meshData.Faces.Count);

            for (int i = 0; i < meshData.Vertices.Count; i++)
                _vertices.Add(new VertexWithFaceBindings(meshData.Vertices[i]));

            foreach (var face in meshData.Faces)
            {
                if (face.Count != 3)
                    throw new ArgumentException($"{nameof(SimpleMesh)} supports only triangulated meshes.");

                _faces.Add(new IndexedTriangle(new VertexRef(face[0]), new VertexRef(face[1]), new VertexRef(face[2])));
            }

            foreach (var face in _faces)
            {
                var triangle = face.Value;
                _vertices[triangle.Vertex1].ConnectedFaces.Add(face.Key);
                _vertices[triangle.Vertex2].ConnectedFaces.Add(face.Key);
                _vertices[triangle.Vertex3].ConnectedFaces.Add(face.Key);
            }

            RecalculateNormals();
        }

        public OperationSnapshot ContractEdge(IndexedEdge indexedEdge, Vector3 newPosition)
        {
            var v1Ref = indexedEdge.Vertex1;
            var v2Ref = indexedEdge.Vertex2;
            var v1 = _vertices[v1Ref];
            var v2 = _vertices[v2Ref];

            var operationSnapshot = new OperationSnapshot();

            foreach (var faceRef in v1.ConnectedFaces.Intersect(v2.ConnectedFaces).ToList())
            {
                var face = _faces[faceRef];

                operationSnapshot.Snapshot(faceRef, _vertices, _faces);

                _vertices[face.Vertex1].ConnectedFaces.Remove(faceRef);
                _vertices[face.Vertex2].ConnectedFaces.Remove(faceRef);
                _vertices[face.Vertex3].ConnectedFaces.Remove(faceRef);

                CheckVertex(face.Vertex1);
                CheckVertex(face.Vertex2);
                CheckVertex(face.Vertex3);

                _faces.Remove(faceRef);
            }

            foreach (var faceRef in v1.ConnectedFaces.Concat(v2.ConnectedFaces).Distinct().ToArray())
            {
                var face = _faces[faceRef];
                operationSnapshot.Snapshot(faceRef, _vertices, _faces);

                if (face.HasVertex(v2Ref))
                {
                    face.ReplaceVertex(v2Ref, v1Ref);
                    _faces[faceRef] = face;

                    _vertices[v1Ref].ConnectedFaces.Add(faceRef);
                    _vertices[v2Ref].ConnectedFaces.Remove(faceRef);
                    CheckVertex(v1Ref);
                }
            }

            CheckVertex(v1Ref);
            CheckVertex(v2Ref);

            if (_vertices.ContainsKey(v1Ref))
            {
                v1.Position = newPosition;
                _vertices[v1Ref] = v1;
            }
           
            return operationSnapshot;
        }

        public void RevertChanges(OperationSnapshot operationSnapshot)
        {
            foreach (var v in operationSnapshot.Vertices)
            {
                if (_vertices[v.Key].IsDeleted())
                {
                    v.Value.SetDeleted(false);
                    _vertices.Respawn(v.Key, v.Value);
                }
                else
                {
                    _vertices[v.Key] = v.Value;
                }
            }

            foreach (var f in operationSnapshot.Faces)
            {
                if (_faces.ContainsKey(f.Key))
                {
                    _faces[f.Key] = f.Value;
                }
                else
                {
                    _faces.Respawn(f.Key, f.Value);
                }

                CheckVertex(f.Value.Vertex1);
                CheckVertex(f.Value.Vertex2);
                CheckVertex(f.Value.Vertex3);
            }
        }

        public IEnumerable<IndexedEdge> GetVertexEdges(VertexRef vertexRef)
        {
            HashSet<VertexRef> duplicates = new HashSet<VertexRef>();
            foreach (var face in _vertices[vertexRef].ConnectedFaces)
            {
                var f = _faces[face];
                if (f.Edge1.ContainsVertex(vertexRef))
                    duplicates.Add(f.Edge1.Vertex1 == vertexRef ? f.Edge1.Vertex2 : f.Edge1.Vertex1);
                if (f.Edge2.ContainsVertex(vertexRef))
                    duplicates.Add(f.Edge2.Vertex1 == vertexRef ? f.Edge2.Vertex2 : f.Edge2.Vertex1);
                if (f.Edge3.ContainsVertex(vertexRef))
                    duplicates.Add(f.Edge3.Vertex1 == vertexRef ? f.Edge3.Vertex2 : f.Edge3.Vertex1);
            }
            foreach (var d in duplicates)
                yield return new IndexedEdge(vertexRef, d);
        }

        public void RecalculateNormals()
        {
            foreach (var pair in _vertices)
            {
                var vert = pair.Value;
                vert.Normal = SurfaceNormals.GetVertexNormal(this, pair.Key);
                _vertices[pair.Key] = vert;
            }
        }

        public void CleanVertices()
        {
            foreach(var pair in _vertices)
            {
                if (pair.Value.ConnectedFaces.Count == 0)
                    _vertices.Remove(pair.Key);
            }

        }

        private void CheckVertex(VertexRef vertexRef)
        {
            var vert = _vertices[vertexRef];
            if (vert.ConnectedFaces.Count == 0 && _vertices.ContainsKey(vertexRef))
                _vertices.Remove(vertexRef);
            else if (vert.ConnectedFaces.Count > 0 && vert.IsDeleted())
            {
                vert.SetDeleted(false);
                _vertices.Respawn(vertexRef, vert);
            }
        }
    }
}
