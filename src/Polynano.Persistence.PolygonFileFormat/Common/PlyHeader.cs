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
using System;
using System.Collections.Generic;

namespace Polynano.Persistence.PolygonFileFormat.Common
{
    public class PlyHeader
    {
        private PlyFormat _format;
        public PlyFormat Format 
        {
            get => _format;
            set 
            {
                if (!Enum.IsDefined(typeof(PlyFormat), value)) 
                    throw new ArgumentException("Invalid enum value.", nameof(Format));
                
                _format = value;
            }
        }

        private IList<PlyElement> _elements;
        public IList<PlyElement> Elements 
        {
            get => _elements;
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Element list must not be null.", nameof(Elements));
                
                if (value.Count == 0)
                    throw new ArgumentException("Header must define at least one element.");

                _elements = value;
            }
        }

        public string Comment { get; set; }
        public string ObjectInfo { get; set; }

        public PlyHeader(PlyFormat format, IList<PlyElement> elements) 
        {
            Format = format;
            Elements = elements;
        }

        public PlyHeader(PlyFormat format, params PlyElement[] elements)
            : this(format, new List<PlyElement>(elements))
        {
        }

        public PlyHeader(PlyFormat format, string comment, string objectInfo, IList<PlyElement> elements) 
        {
            Format = format;
            Comment = comment;
            ObjectInfo = objectInfo;
            Elements = elements;
        }
    }
}