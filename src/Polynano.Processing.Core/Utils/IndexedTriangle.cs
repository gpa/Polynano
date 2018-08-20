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
using System;

namespace Polynano.Processing.Core.Utils
{
    public struct IndexedTriangle : IDeleted
    {
        public VertexRef Vertex1;

        public VertexRef Vertex2;

        public VertexRef Vertex3;

        public IndexedTriangle(VertexRef v1, VertexRef v2, VertexRef v3)
        {
            if ((v1 == v2 || v1 == v3 || v2 == v3) && !v1.IsNone())
            {
                throw new ArgumentException("Invalid triangle.");
            }

            Vertex1 = v1;
            Vertex2 = v2;
            Vertex3 = v3;
        }

        public bool HasVertex(VertexRef v)
        {
            return Vertex1 == v || Vertex2 == v || Vertex3 == v;
        }

        public void ReplaceVertex(VertexRef v, VertexRef nv)
        {
            if (nv == Vertex1 || nv == Vertex2 || nv == Vertex3)
            {
                throw new ArgumentException($"This triangle already contains vertex {nv}.");
            }

            if (Vertex1 == v)
                Vertex1 = nv;
            else if (Vertex2 == v)
                Vertex2 = nv;
            else if (Vertex3 == v)
                Vertex3 = nv;
            else
                throw new ArgumentException($"This triangle does not contain vertex {v}");
        }

        public bool IsDeleted()
        {
            return Vertex1.IsNone() && Vertex2.IsNone() && Vertex3.IsNone();
        }

        public object GetDeletedClone()
        {
            return new IndexedTriangle(VertexRef.None, VertexRef.None, VertexRef.None);
        }

        public IndexedEdge Edge1 => new IndexedEdge(Vertex1, Vertex2);

        public IndexedEdge Edge2 => new IndexedEdge(Vertex2, Vertex3);

        public IndexedEdge Edge3 => new IndexedEdge(Vertex3, Vertex1);

        public static int SizeInBytes => sizeof(int) * 3;
    }
}