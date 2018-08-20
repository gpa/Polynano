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
using OpenTK;
using Polynano.DemoApplication.Extensions;
using Polynano.Processing.Core;
using Polynano.Processing.Core.Geometry;
using Polynano.Processing.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Polynano.DemoApplication.Utils
{
    public class ModelData
    {
        public Vector3[] Vertices { get; set; }

        public Vector3[] Normals { get; set; }

        public int[] FaceIndices { get; set; }

        public int[] EdgeIndices { get; set; }

        public AABB AABB { get; set; }

        public static ModelData CreateFrom(TraversableMesh traversableMesh)
        {
            var indices = new int[traversableMesh.Faces.Count * 3];
            var j = 0;
            int maxIndex = 0;
            foreach (var faceRef in traversableMesh.Faces.Keys)
            {
                var faceIndices = traversableMesh.EnumerateVertices(faceRef).ToList();

                if (faceIndices.Count != 3)
                    throw new NotSupportedException();

                maxIndex = Math.Max(maxIndex, faceIndices[0].Index);
                maxIndex = Math.Max(maxIndex, faceIndices[1].Index);
                maxIndex = Math.Max(maxIndex, faceIndices[2].Index);

                indices[j] = faceIndices[0].Index;
                indices[j + 1] = faceIndices[1].Index;
                indices[j + 2] = faceIndices[2].Index;
                j += 3;
            }

            var vertices = new OpenTK.Vector3[maxIndex + 1];
            var normals = new OpenTK.Vector3[maxIndex + 1];

            foreach (var halfedgeVertex in traversableMesh.Vertices)
            {
                vertices[halfedgeVertex.Key.Index] = halfedgeVertex.Value.Position.ToOpenToolkitVector();
                normals[halfedgeVertex.Key.Index] = SurfaceNormals.GetVertexNormal(traversableMesh, halfedgeVertex.Key).ToOpenToolkitVector();
            }

            var edgeIndices = new int[traversableMesh.Edges.Count * 2];
            int k = 0;
            foreach (var edge in traversableMesh.Edges)
            {
                edgeIndices[k] = edge.Vertex1.Index;
                edgeIndices[k + 1] = edge.Vertex2.Index;
                k += 2;
            }

            var renderingData = new ModelData
            {
                Vertices = vertices,
                Normals = normals,
                FaceIndices = indices,
                EdgeIndices = edgeIndices,
                AABB = AABB.GetAABBFor(traversableMesh.Vertices.Select(x => x.Value.Position))
            };
            return renderingData;
        }

        public static ModelData CreateFrom(SimpleMesh simpleMesh)
        {
            var faceIndices = new int[simpleMesh.Faces.Count * 3];
            HashSet<IndexedEdge> uniqueEdges = new HashSet<IndexedEdge>();
            var i = 0;
            int maxIndex = 0;
            foreach (var face in simpleMesh.Faces)
            {
                faceIndices[i] = face.Value.Vertex1.Index;
                faceIndices[i+1] = face.Value.Vertex2.Index;
                faceIndices[i+2] = face.Value.Vertex3.Index;

                maxIndex = Math.Max(maxIndex, faceIndices[i]);
                maxIndex = Math.Max(maxIndex, faceIndices[i+1]);
                maxIndex = Math.Max(maxIndex, faceIndices[i+2]);

                uniqueEdges.Add(face.Value.Edge1);
                uniqueEdges.Add(face.Value.Edge2);
                uniqueEdges.Add(face.Value.Edge3);

                i += 3;
            }

            var vertices = new OpenTK.Vector3[maxIndex+1];
            var normals = new OpenTK.Vector3[maxIndex+1];

            foreach (var vertex in simpleMesh.Vertices)
            {
                vertices[vertex.Key.Index] = vertex.Value.Position.ToOpenToolkitVector();
                normals[vertex.Key.Index] = vertex.Value.Normal.ToOpenToolkitVector();
            }

            var edgeIndices = new int[uniqueEdges.Count * 2];
            i = 0;
            foreach(var edge in uniqueEdges)
            {
                edgeIndices[i] = edge.Vertex1.Index;
                edgeIndices[i+1] = edge.Vertex2.Index;
                i += 2;
            }

            var renderingData = new ModelData()
            {
                Vertices = vertices,
                Normals = normals,
                FaceIndices = faceIndices,
                EdgeIndices = edgeIndices,
                AABB = AABB.GetAABBFor(simpleMesh.Vertices.Select(x => x.Value.Position))
            };
            return renderingData;
        }
    }
}
