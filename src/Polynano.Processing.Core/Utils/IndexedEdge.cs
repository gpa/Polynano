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
    public struct IndexedEdge : IDeleted
    {
        public VertexRef Vertex1;

        public VertexRef Vertex2;

        public IndexedEdge(VertexRef v1, VertexRef v2)
        {
            if (v1 == v2)
            {
                throw new ArgumentException();
            }

            Vertex1 = v1.Index < v2.Index ? v1 : v2;
            Vertex2 = Vertex1 == v1 ? v2 : v1;
        }

        public bool ContainsVertex(VertexRef v)
        {
            return Vertex1 == v || Vertex2 == v;
        }

        public bool IsDeleted()
        {
            return Vertex1.IsNone() && Vertex2.IsNone();
        }

        public object GetDeletedClone()
        {
            return new IndexedEdge(VertexRef.None, VertexRef.None);
        }

        public static int SizeInBytes => sizeof(int) * 2;
    }
}