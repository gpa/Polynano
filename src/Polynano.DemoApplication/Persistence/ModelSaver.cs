using OpenTK;
using Polynano.DemoApplication.Utils;
using Polynano.Persistence.PolygonFileFormat.Common;
using Polynano.Persistence.PolygonFileFormat.Writers;
using System;
using System.IO;

namespace Polynano.DemoApplication.Persistence
{
    public class ModelSaver
    {
        public IProgress<int> OnProgress { get; set; }

        public void Save(string filePath, ModelData data)
        {
            var elements = new[]
            {
                new PlyElement("vertex", data.Vertices.Length, new[]
                {
                    new PlyProperty("x", PlyType.Float),
                    new PlyProperty("y", PlyType.Float),
                    new PlyProperty("z", PlyType.Float)
                }),
                new PlyElement("face", data.FaceIndices.Length / 3, new[]
                {
                    new PlyArrayProperty("vertex_index", PlyType.Int, PlyType.Int)
                })
            };

            var header = new PlyHeader(PlyFormat.BinaryLittleEndian, "Simplified by polynano", null, elements);
            var writer = new PlyBinaryWriter(new FileStream(filePath, FileMode.Create), header);

            long totalToWrite = data.FaceIndices.Length / 3 + data.Vertices.Length;
            long written = 0;

            int lastReportedProgress = 10;

            // Go though the vertices and write them to the file.
            foreach (Vector3 vertex in data.Vertices)
            {
                writer.WriteValues(vertex.X, vertex.Y, vertex.Z);
                written++;

                // report progress on every 25% percent
                int progress = (int)(written * 100 / totalToWrite);
                if (progress > lastReportedProgress + 25 && OnProgress != null)
                {
                    OnProgress.Report(progress);
                    lastReportedProgress = progress;
                }
            }

            for(int i = 0; i < data.FaceIndices.Length; i += 3)
            {
                writer.WriteArray(data.FaceIndices[i], data.FaceIndices[i + 1], data.FaceIndices[i + 2]);
                written++;
                int progress = (int)(written * 100 / totalToWrite);
                if (progress > lastReportedProgress + 25 && OnProgress != null)
                {
                    OnProgress.Report(progress);
                    lastReportedProgress = progress;
                }
            }

            OnProgress?.Report(100);
            writer.Dispose();
        }
    }
}
