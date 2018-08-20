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
using System.Collections.Generic;
using System.Numerics;

namespace Polynano.Processing.Core.Tests
{
    class TraversableMeshTestBase
    {
        protected List<Vector3> Vertices { get; set; }
        protected FaceCollection Faces { get; set; }

        protected void AddDummyVertices(int count)
        {
            for (int i = 0; i < count; i++)
                Vertices.Add(new Vector3());
        }

        protected void AddFace(params int[] indices)
        {
            Faces.AddFace(indices);
        }

        protected TraversableMesh GetTraversableMesh()
        {
            var normals = new List<Vector3>();
            var mesh = new MeshData(Vertices, normals, Faces);
            return new TraversableMesh(mesh);
        }

        protected MutableTraversableMesh GetMutableTraversableMesh()
        {
            var normals = new List<Vector3>();
            var mesh = new MeshData(Vertices, normals, Faces);
            return new MutableTraversableMesh(mesh);
        }
    }
}
