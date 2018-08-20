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
using Polynano.DemoApplication.Extensions;
using Polynano.DemoApplication.Exceptions;
using Polynano.Persistence.PolygonFileFormat.Common;
using System;

namespace Polynano.DemoApplication.Persistence
{
    internal class PlyHeaderNormalizer
    {
        public PlyHeader PlyHeader { get; private set; }

        public PlyHeaderNormalizer(PlyHeader header)
        {
            if (header == null)
                throw new ArgumentNullException(nameof(header));

            PlyHeader = header;
        }

        public int FindFaceCount()
        {
            foreach(var element in PlyHeader.Elements)
            {
                if (DeclaresFaces(element))
                    return element.InstanceCount;
            }

            throw new FailedToLoadFileException("Failed to find the face element");
        }

        public int FindVertexCount()
        {
            foreach (var element in PlyHeader.Elements)
            {
                if (DeclaresVertices(element))
                    return element.InstanceCount;
            }

            throw new FailedToLoadFileException("Failed to find the vertex element");
        }

        public bool DeclaresVertices(PlyElement element)
        {
            return element.Name.ToLower().IsOneOf("vertex", "vertices", "points");
        }

        public bool DeclaresFaces(PlyElement element)
        {
            return element.Name.ToLower().IsOneOf("faces", "face", "triangles");
        }

        public bool DeclaresNormals(PlyElement element)
        {
            return element.Name.ToLower().IsOneOf("normals", "normal");
        }

        public bool DeclaresVertexCollection(PlyProperty property)
        {
            return property.Name.ToLower().IsOneOf("vertex_indices", "vertex", "vertices", "vertex_indexes", 
                "vertex_index", "index");
        }

        public bool DeclaresXAxisValue(PlyProperty property)
        {
            return property.Name.ToLower().ToLower() == "x";
        }

        public bool DeclaresYAxisValue(PlyProperty property)
        {
            return property.Name.ToLower() == "y";
        }

        public bool DeclaresZAxisValue(PlyProperty property)
        {
            return property.Name.ToLower() == "z";
        }

        public bool DeclaresXAxisNormalValue(PlyProperty property)
        {
            return property.Name.ToLower().IsOneOf("nx", "normal_x", "x_normal", "xnormal", "normalx");
        }

        public bool DeclaresYAxisNormalValue(PlyProperty property)
        {
            return property.Name.ToLower().IsOneOf("ny", "normal_y", "y_normal", "ynormal", "normaly");
        }

        public bool DeclaresZAxisNormalValue(PlyProperty property)
        {
            return property.Name.ToLower().IsOneOf("nz", "normal_z", "z_normal", "znormal", "normalz");
        }
    }
}
