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
using OpenTK.Graphics.OpenGL;
using System;
using System.Diagnostics;

namespace Polynano.Rendering
{
    public class Shader : IDisposable
    {
        private int _nativeHandle;

        public Shader(ShaderType type, string source)
        {
            _nativeHandle = GL.CreateShader(type);

            GL.ShaderSource(_nativeHandle, source);
            GL.CompileShader(_nativeHandle);

            Debug.WriteLine(GL.GetShaderInfoLog(_nativeHandle));
        }

        public void Attach(int vertexArrayId)
        {
            GL.AttachShader(vertexArrayId, _nativeHandle);
        }

        public void Detach(int vertexArrayId)
        {
            GL.DetachShader(vertexArrayId, _nativeHandle);
        }

        public void Dispose()
        {
            if (_nativeHandle != 0)
            {
                GL.DeleteShader(_nativeHandle);
                _nativeHandle = 0;
            }
        }
    }
}
