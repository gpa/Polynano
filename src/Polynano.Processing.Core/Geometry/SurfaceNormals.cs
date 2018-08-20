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
using System.Linq;
using System.Numerics;

namespace Polynano.Processing.Core.Geometry
{
    public static class SurfaceNormals
    {
        public static Vector3 GetVertexNormal(TraversableMesh traversableMesh, VertexRef vertexRef)
        {
            Vector3 vertexNormal = new Vector3();
            var i = 0;
            foreach (var halfedgeRef in traversableMesh.EnumerateOneRing(vertexRef))
            {
                if (i >= 3)
                    break;

                var faceRef = traversableMesh.Halfedges[halfedgeRef].Face;

                if (faceRef.IsNone())
                    continue;

                var faceNormal = GetFaceNormal(traversableMesh, faceRef);
                vertexNormal += faceNormal;
                i++;
            }

            return vertexNormal / 3;
        }

        public static Vector3 GetVertexNormal(SimpleMesh simpleMesh, VertexRef vertexRef)
        {
            Vector3 vertexNormal = new Vector3();
            var i = 0;

            foreach(var faceRef in simpleMesh.Vertices[vertexRef].ConnectedFaces)
            {
                var faceNormal = GetFaceNormal(simpleMesh, faceRef);
                vertexNormal += faceNormal;
                i++;
            }

            return vertexNormal / 3;
        }

        public static Vector3 GetFaceNormal(TraversableMesh traversableMesh, FaceRef faceRef)
        {
            var vertices = traversableMesh.EnumerateVertices(faceRef).Take(3).Select(x => traversableMesh.Vertices[x].Position).ToList();
            return new Triangle(vertices[0], vertices[1], vertices[2]).Normal;
        }

        public static Vector3 GetFaceNormal(SimpleMesh simpleMesh, FaceRef faceRef)
        {
            var triangle = simpleMesh.Faces[faceRef];
            var v0 = simpleMesh.Vertices[triangle.Vertex1].Position;
            var v1 = simpleMesh.Vertices[triangle.Vertex2].Position;
            var v2 = simpleMesh.Vertices[triangle.Vertex3].Position;
            return new Triangle(v0, v1, v2).Normal;
        }
    }
}