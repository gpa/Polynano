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
using Polynano.DemoApplication.Extensions;
using Polynano.DemoApplication.Utils;
using Polynano.Processing.Core;
using Polynano.Processing.Core.Geometry;
using Polynano.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Polynano.Startup.Utils
{
    public class MeshViewControlController : IDisposable
    {
        private MeshViewControl _meshViewControl;

        private ShaderProgram _shaderProgram;

        private FaceMesh _faceMesh;
        private EdgeMesh _edgeMesh;
        private VertexMesh _vertexMesh;

        private bool _displayFaces;
        public bool DisplayFaces
        {
            get => _displayFaces;
            set
            {
                _displayFaces = value;
                UpdateView();
            }
        }

        private bool _displayEdges;
        public bool DisplayEdges
        {
            get => _displayEdges;
            set
            {
                _displayEdges = value;
                UpdateView();
            }
        }

        private bool _displayVertices;
        public bool DisplayVertices
        {
            get => _displayVertices;
            set
            {
                _displayVertices = value;
                UpdateView();
            }
        }

        public OpenTK.Vector3 FaceMeshColor { get; set; } = new OpenTK.Vector3(0.6f, 0.0f, 0.0f);
        public OpenTK.Vector3 EdgeMeshColor { get; set; } = new OpenTK.Vector3(1.0f, 1.0f, 1.0f);
        public OpenTK.Vector3 VertexMeshColor { get; set; } = new OpenTK.Vector3(0.0f, 1.0f, 0.0f);

        public MeshViewControlController(MeshViewControl meshViewControl)
        {
            _meshViewControl = meshViewControl;
            _displayFaces = true;
        }

        public void Initialize()
        {
            var vertexShaderSource = File.ReadAllText("shaders\\flatShader.vert");
            var fragmentShaderSource = File.ReadAllText("shaders\\flatShader.frag");

            using (var vertexShader = new Shader(ShaderType.VertexShader, vertexShaderSource))
            {
                using (var fragmentShader = new Shader(ShaderType.FragmentShader, fragmentShaderSource))
                {
                    _shaderProgram = new ShaderProgram(vertexShader, fragmentShader);
                    _meshViewControl.RenderStates.ShaderProgram = _shaderProgram;
                }
            }
        }

        public void Resize(int offsetX, int offsetY, int width, int height)
        {
            _meshViewControl.Location = new Point(offsetX, offsetY);
            _meshViewControl.Width = width;
            _meshViewControl.Height = height;
            _meshViewControl.Refresh();
        }

        private void UpdateView()
        {
            var drawables = new List<IDrawable>(3);

            if (DisplayFaces)
                drawables.Add(_faceMesh);

            if (DisplayEdges)
                drawables.Add(_edgeMesh);

            if (DisplayVertices)
                drawables.Add(_vertexMesh);

            _meshViewControl.Mesh = new CombinedDrawable(drawables);
            _meshViewControl.Refresh();
        }

        public void SetMesh(TraversableMesh traversableMesh)
        {
            ClearMeshes();

            var renderingData = ModelData.CreateFrom(traversableMesh);
            BuildMesh(renderingData);

            UpdateView();
        }

        public void SetMesh(SimpleMesh simpleMesh)
        {
            ClearMeshes();

            var renderingData = ModelData.CreateFrom(simpleMesh);
            BuildMesh(renderingData);

            UpdateView();
        }

        private void BuildMesh(ModelData meshData)
        {
            float rx = 2.0f / meshData.AABB.Size.X;
            float ry = 2.0f / meshData.AABB.Size.Y;
            float rz = 2.0f / meshData.AABB.Size.Z;
            float factor = Math.Min(rx, Math.Min(ry, rz));

            _faceMesh = new FaceMesh(meshData.Vertices, meshData.Normals, meshData.FaceIndices);

            _faceMesh.Translate((new Vector3() - meshData.AABB.Center).ToOpenToolkitVector());
            _faceMesh.Scale(factor);

            _edgeMesh = new EdgeMesh(_faceMesh, meshData.EdgeIndices);
            _vertexMesh = new VertexMesh(_faceMesh, meshData.Vertices.Length);

            _faceMesh.Color = FaceMeshColor;
            _edgeMesh.Color = EdgeMeshColor;
            _vertexMesh.Color = VertexMeshColor;
        }

        void ClearMeshes()
        {
            _faceMesh?.Dispose();
            _edgeMesh?.Dispose();
            _vertexMesh?.Dispose();

            _faceMesh = null;
            _edgeMesh = null;
            _vertexMesh = null;
        }

        public void Dispose()
        {
            _shaderProgram.Dispose();
            ClearMeshes();
        }
    }
}