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
using Polynano.DemoApplication.Persistence;
using Polynano.DemoApplication.Utils;
using Polynano.Processing.Core;
using Polynano.Processing.Simplification;
using System;

namespace Polynano.Startup.ViewModels
{
    public class ApplicationViewModel : IDisposable
    {
        public SimpleMesh ProcessingMesh { get; private set; }

        public SimpleMeshSimplifier MeshSimplifier { get; private set; }

        public int OriginalVertexCount { get; private set; }

        public string OriginalFileName { get; private set; }

        public void Load(string filePath, IProgress<int> progressReporter = null)
        {
            var modelLoader = new ModelLoader
            {
                OnProgress = progressReporter
            };

            var mesh = modelLoader.Load(filePath);
            ProcessingMesh = new SimpleMesh(mesh.Model);
            MeshSimplifier = new SimpleMeshSimplifier(ProcessingMesh);
            OriginalVertexCount = ProcessingMesh.Vertices.Count;
            OriginalFileName = mesh.FileName;
        }

        public void Save(string filePath, IProgress<int> progressReporter = null)
        {
            var writer = new ModelSaver()
            {
                OnProgress = progressReporter
            };

            writer.Save(filePath, ModelData.CreateFrom(ProcessingMesh));
        }

        public bool Simplify(float ratio, long maxIterations = long.MaxValue)
        {
            var targetVertexCount = OriginalVertexCount * (ratio / 100.0f);
            var reduce = targetVertexCount < ProcessingMesh.Vertices.Count;

            if (reduce)
            {
                for (long i = 0; i < maxIterations; ++i)
                {
                    if (targetVertexCount >= ProcessingMesh.Vertices.Count)
                        return true;

                    if (!MeshSimplifier.SimplifyOneStep())
                        return true;
                }
            }
            else
            {
                for (long i = 0; i < maxIterations; ++i)
                {
                    if (targetVertexCount <= ProcessingMesh.Vertices.Count)
                    {
                        if (targetVertexCount == OriginalVertexCount && MeshSimplifier.HasSnapshots())
                        {
                            while (MeshSimplifier.RevertOneStep());
                        }

                        return true;
                    }

                    if (!MeshSimplifier.RevertOneStep())
                        return true;
                }
            }

            return false;
        }

        public void InitializeSimplifier()
        {
            MeshSimplifier.Initialize();
        }

        public void Dispose()
        {
        }
    }
}
