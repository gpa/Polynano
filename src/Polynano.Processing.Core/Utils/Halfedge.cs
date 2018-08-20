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
namespace Polynano.Processing.Core.Utils
{
    public struct Halfedge : IDeleted
    {
        public VertexRef Vertex { get; }

        public FaceRef Face { get; }

        public HalfedgeRef Next { get; }

        public Halfedge(VertexRef vertex, FaceRef face, HalfedgeRef nextHalfedge)
        {
            Vertex = vertex;
            Face = face;
            Next = nextHalfedge;
        }

        internal Halfedge(Halfedge halfedge, FaceRef faceRef)
        {
            Vertex = halfedge.Vertex;
            Next = halfedge.Next;
            Face = faceRef;
        }

        internal Halfedge(Halfedge halfedge, VertexRef vertexRef)
        {
            Vertex = vertexRef;
            Face = halfedge.Face;
            Next = halfedge.Next;
        }

        internal Halfedge(Halfedge halfedge, HalfedgeRef halfedgeRef)
        {
            Vertex = halfedge.Vertex;
            Face = halfedge.Face;
            Next = halfedgeRef;
        }

        public override string ToString()
        {
            return $"v: {Vertex.Index} f: {Face.Index} n: {Next.Index}";
        }

        public bool IsDeleted()
        {
            return Vertex.IsNone();
        }

        public object GetDeletedClone()
        {
            return new Halfedge(VertexRef.None, Face, Next);
        }
    }
}