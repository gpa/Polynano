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
using Polynano.Persistence.PolygonFileFormat;
using Polynano.Processing.Core;
using Polynano.Processing.Core.Collections;
using Polynano.Processing.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Polynano.DemoApplication.Persistence
{
    public class ModelLoader
    {
        public IProgress<int> OnProgress { get; set; }

        public ModelWithMetadata<MeshData> Load(string filePath)
        {
            if (filePath.ToLower().EndsWith(".ply"))
                return LoadPly(filePath);

            throw new ArgumentException("Unrecognized file format extension.", nameof(filePath));
        }

        private ModelWithMetadata<MeshData> LoadPly(string filePath)
        {
            MeshData mesh;
            bool hasVectorNormals = false;
            using (var plyModelReader = new PlyModelLoader(filePath))
            {
                var plyHeader = plyModelReader.ContentHeader;
                var plyReader = plyModelReader.ContentReader;
                var plyHeaderNormalizer = new PlyHeaderNormalizer(plyHeader);
                var vertexCount = plyHeaderNormalizer.FindVertexCount();
                var faceCount = plyHeaderNormalizer.FindFaceCount();

                var vertices = new List<Vector3>(vertexCount);
                var faces = new FaceCollection(faceCount, 3);

                long totalElementsToLoad = vertexCount + faceCount;
                int lastProgressReported = 10;

                Vector3 tmpVector;
                List<VertexRef> tmpFace = new List<VertexRef>(5);
                foreach (var element in plyHeader.Elements)
                {
                    for (var i = 0; i < element.InstanceCount; ++i)
                    {
                        tmpFace.Clear();
                        tmpVector = new Vector3();

                        int progress = (int)((vertices.Count + faces.Count) * 100 / totalElementsToLoad);
                        if (progress > lastProgressReported + 10 && OnProgress != null)
                        {
                            OnProgress.Report(progress);
                            lastProgressReported = progress;
                        }

                        if (plyHeaderNormalizer.DeclaresVertices(element))
                        {
                            foreach (var property in element.Properties)
                            {
                                if (plyHeaderNormalizer.DeclaresXAxisValue(property))
                                    tmpVector.X = plyReader.ReadProperty<float>();
                                else if (plyHeaderNormalizer.DeclaresYAxisValue(property))
                                    tmpVector.Y = plyReader.ReadProperty<float>();
                                else if (plyHeaderNormalizer.DeclaresZAxisValue(property))
                                    tmpVector.Z = plyReader.ReadProperty<float>();
                                else
                                    plyReader.SkipProperty();
                            }
                            vertices.Add(tmpVector);
                        }
                        else if (plyHeaderNormalizer.DeclaresFaces(element))
                        {
                            foreach (var property in element.Properties)
                            {
                                if (plyHeaderNormalizer.DeclaresVertexCollection(property))
                                {
                                    var indices = plyReader.ReadArray<int>().ToList();
                                    if (indices.Count == 4)
                                    {
                                        var face1 = new int[] { indices[0], indices[1], indices[2] };
                                        var face2 = new int[] { indices[0], indices[2], indices[3] };
                                        faces.AddFace(face1);
                                        faces.AddFace(face2);
                                    }
                                    else
                                    {
                                        faces.AddFace(indices);
                                    }
                                }
                                else
                                    plyReader.SkipProperty();
                            }
                        }
                    }
                }

                mesh = new MeshData(vertices, new List<Vector3>(), faces);
            }

            OnProgress?.Report(100);

            var modelWithMetadata = new ModelWithMetadata<MeshData>();
            modelWithMetadata.Model = mesh;
            modelWithMetadata.FileName = filePath;
            modelWithMetadata.HasNormalVectors = hasVectorNormals;
            return modelWithMetadata;
        }
    }
}
