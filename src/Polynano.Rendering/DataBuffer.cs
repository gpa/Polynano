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
    public class DataBuffer : IDisposable
    {
        private int _nativeHandle;

        public BufferTarget BufferTarget { get; private set; }

        public BufferUsageHint BufferUsageHint { get; private set; }

        public DataBuffer()
        {
            _nativeHandle = GL.GenBuffer();
        }

        public void BufferData<T>(
            BufferTarget bufferTarget,
            T[] data,
            int totalSizeInBytes,
            BufferUsageHint bufferUsageHint = BufferUsageHint.DynamicDraw)
            where T : struct
        {
            BufferTarget = bufferTarget;
            BufferUsageHint = bufferUsageHint;
            Bind();
            GL.BufferData<T>(bufferTarget, new IntPtr(totalSizeInBytes), data, bufferUsageHint);
            Unbind();
        }

        public void BufferSubData<T>(
            BufferTarget bufferTarget,
            T[] data,
            int totalSizeInBytes,
            IntPtr offset)
            where T : struct
        {
            Bind();
            GL.BufferSubData<T>(bufferTarget, offset, totalSizeInBytes, data);
            Unbind();
        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget, _nativeHandle);
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget, 0);
        }

        public void BindAsStandardVertexBuffer(int attributeId)
        {
            GL.EnableVertexAttribArray(attributeId);
            GL.BindBuffer(BufferTarget, _nativeHandle);
            GL.VertexAttribPointer(attributeId, 3, VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0);
        }

        public void Dispose()
        {
            if (_nativeHandle != 0)
            {
                GL.DeleteBuffer(_nativeHandle);
                _nativeHandle = 0;
            }
        }
    }
}
