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
using System.Collections.Generic;

namespace Polynano.Processing.Core.Utils
{
    public class OperationSnapshot
    {
        public Dictionary<VertexRef, VertexWithFaceBindings> Vertices;

        public Dictionary<FaceRef, IndexedTriangle> Faces;

        public OperationSnapshot()
        {
            Vertices = new Dictionary<VertexRef, VertexWithFaceBindings>();
            Faces = new Dictionary<FaceRef, IndexedTriangle>();
        }

        public void Snapshot(FaceRef faceRef, IReadOnlyDictionary<VertexRef, VertexWithFaceBindings> vertices, IReadOnlyDictionary<FaceRef, IndexedTriangle> faces)
        {
            void snapshotVertex(VertexRef vertexRef)
            {
                if (!Vertices.ContainsKey(vertexRef))
                    Vertices.Add(vertexRef, vertices[vertexRef].Clone());
            }

            if (!Faces.ContainsKey(faceRef))
            {
                var face = faces[faceRef];
                Faces.Add(faceRef, face);
                snapshotVertex(face.Vertex1);
                snapshotVertex(face.Vertex2);
                snapshotVertex(face.Vertex3);
            }
        }
    }
}
