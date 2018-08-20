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
using System;

namespace Polynano.Rendering
{
    public abstract class Mesh : ITransformable, IDrawable, IDisposable
    {
        protected VertexArray _vertexArray;

        protected DataBuffer _vertexBuffer;

        protected int _indexCount;

        public Matrix4 Transformation { get; protected set; } = Matrix4.Identity;

        public Vector3 Color { get; set; } = new Vector3(0.6f, 0, 0);

        public Mesh()
        {
            _vertexArray = new VertexArray();
            _vertexBuffer = new DataBuffer();
        }

        public abstract void Draw(RenderTarget renderTarget, RenderStates renderStates);

        public void Scale(float scale)
        {
            Transformation *= Matrix4.CreateScale(scale);
        }

        public void Translate(Vector3 delta)
        {
            Transformation *= Matrix4.CreateTranslation(delta);
        }

        protected void PushState(RenderStates renderStates)
        {
            _vertexArray.Bind();
            renderStates.ShaderProgram.MeshColor = Color;
            renderStates.ShaderProgram.ModelMatrix = Transformation;
        }

        internal DataBuffer GetVertexBuffer()
        {
            return _vertexBuffer;
        }

        public virtual void Dispose()
        {
            _vertexArray.Dispose();
            _vertexBuffer.Dispose();
        }
    }
}