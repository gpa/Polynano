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
using System.Diagnostics;

namespace Polynano.Rendering
{
    public class ShaderProgram : IShaderProgram
    {
        private const string ModelMatrixName = "model_matrix";
        private const string ViewMatrixName = "view_matrix";
        private const string ProjectionMatrixName = "projection_matrix";
        private const string MeshColorName = "mesh_color";

        private int _modelMatrixLocation;

        private int _viewMatrixLocation;

        private int _projectionMatrixLocation;

        private int _meshColorLocation;

        private int _nativeHandle;

        public Matrix4 ModelMatrix
        {
            set => GL.UniformMatrix4(_modelMatrixLocation, false, ref value);
        }

        public Matrix4 ViewMatrix
        {
            set => GL.UniformMatrix4(_viewMatrixLocation, false, ref value);
        }

        public Matrix4 ProjectionMatrix
        {
            set => GL.UniformMatrix4(_projectionMatrixLocation, false, ref value);
        }

        public Vector3 MeshColor
        {
            set => GL.Uniform3(_meshColorLocation, value);
        }

        public ShaderProgram(params Shader[] shaders)
        {
            _nativeHandle = GL.CreateProgram();

            foreach (var shader in shaders)
                shader.Attach(_nativeHandle);

            GL.LinkProgram(_nativeHandle);
            Debug.WriteLine(GL.GetProgramInfoLog(_nativeHandle));

            foreach (var shader in shaders)
                shader.Detach(_nativeHandle);

            LocateUniforms();
        }

        private void LocateUniforms()
        {
            _modelMatrixLocation = GL.GetUniformLocation(_nativeHandle, ModelMatrixName);
            _viewMatrixLocation = GL.GetUniformLocation(_nativeHandle, ViewMatrixName);
            _projectionMatrixLocation = GL.GetUniformLocation(_nativeHandle, ProjectionMatrixName);
            _meshColorLocation = GL.GetUniformLocation(_nativeHandle, MeshColorName);
            Debug.Assert(_modelMatrixLocation != -1 && _projectionMatrixLocation != -1 && _meshColorLocation != -1);
        }

        public void Bind()
        {
            GL.UseProgram(_nativeHandle);
        }

        public void Unbind()
        {
            GL.UseProgram(0);
        }

        public void Dispose()
        {
            if (_nativeHandle != 0)
            {
                GL.DeleteProgram(_nativeHandle);
                _nativeHandle = 0;
            }
        }
    }
}