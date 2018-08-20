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
using OpenTK.Graphics.OpenGL;
using System;

namespace Polynano.Rendering
{
    public class FaceMesh : Mesh
    {
        private readonly DataBuffer _indexBuffer;

        private readonly DataBuffer _normalBuffer;
        
        public FaceMesh(Vector3[] vertices, Vector3[] normals, int[] indices)
        {
            _indexBuffer = new DataBuffer();
            _normalBuffer = new DataBuffer();
            _indexCount = indices.Length;
            SetMesh(vertices, normals, indices);
        }

        public override void Draw(RenderTarget renderTarget, RenderStates renderStates)
        {
            PushState(renderStates);
            GL.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }

        private void SetMesh(Vector3[] vertices, Vector3[] normals, int[] indices)
        {
            _vertexBuffer.BufferData(BufferTarget.ArrayBuffer, vertices, vertices.Length * Vector3.SizeInBytes);
            _indexBuffer.BufferData(BufferTarget.ElementArrayBuffer, indices, _indexCount * sizeof(int));
            _normalBuffer.BufferData(BufferTarget.ArrayBuffer, normals, vertices.Length * Vector3.SizeInBytes);

            _vertexArray.Bind();
            _vertexBuffer.BindAsStandardVertexBuffer(0);
            _indexBuffer.Bind();
            _normalBuffer.BindAsStandardVertexBuffer(1);
            _vertexArray.Unbind();
        }

        public void UpdateMesh(Vector3[] vertices, int[] indices)
        {
            _vertexArray.Unbind();
            _vertexBuffer.BufferData(BufferTarget.ArrayBuffer, vertices, vertices.Length * Vector3.SizeInBytes);
            _indexBuffer.BufferData(BufferTarget.ElementArrayBuffer, indices, indices.Length * sizeof(int));
            _vertexArray.Bind();
        }

        public override void Dispose()
        {
            _indexBuffer.Dispose();
            _normalBuffer.Dispose();
        }
    }
}
