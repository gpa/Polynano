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
using Polynano.Persistence.PolygonFileFormat.Common;
using System;
using System.IO;
using System.Text;

namespace Polynano.Persistence.PolygonFileFormat.Writers
{
    public class PlyAsciiWriter : PlyWriter
    {
        private const string Whitespace = " ";
        private const string Newline = "\r\n";

        private readonly StreamWriter _writer;

        public PlyAsciiWriter(Stream targetStream, PlyHeader header)
            : base(targetStream, header)
        {
            if (header.Format != PlyFormat.Ascii)
                throw new NotSupportedException($"The {nameof(PlyAsciiWriter)} does not support binary encoding.");
            _writer = new StreamWriter(targetStream, Encoding.Default, 1024, leaveOpen: true);
        }

        protected override void WriteHeaderInternal(string header)
        {
            _writer.Write(header);
        }

        protected override void WriteValueInternal<T>(T value)
        {
            WriteSeparator();
            var valueToWrite = PlyTypeConverter.ValueToString(value);
            _writer.Write(valueToWrite);
        }

        protected override void WriteArrayInternal<T>(params T[] data)
        {
            WriteSeparator();
            var arrayCountPrefix = PlyTypeConverter.ValueToString(data.Length);
            _writer.Write(arrayCountPrefix);

            foreach (T value in data)
            {
                var valueToWrite = PlyTypeConverter.ValueToString(value);
                _writer.Write(Whitespace);
                _writer.Write(valueToWrite);
            }
        }

        private void WriteSeparator()
        {
            if (_iterator.IsOnFirstProperty)
                if(!_iterator.IsOnFirstElement)
                    _writer.Write(Newline);
            else
                _writer.Write(Whitespace);
        }
        
        public override void Dispose()
        {
            _writer.Dispose();
            base.Dispose();
        }
    }
}
