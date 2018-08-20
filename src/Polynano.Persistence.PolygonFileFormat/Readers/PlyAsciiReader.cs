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
using Polynano.Persistence.PolygonFileFormat.Exceptions;
using Polynano.Persistence.PolygonFileFormat.Readers.StreamReaders;
using System;
using System.Collections.Generic;

namespace Polynano.Persistence.PolygonFileFormat.Readers
{
    public class PlyAsciiReader : PlyReader
    {
        protected readonly ITextReader _textReader;

        public PlyAsciiReader(ITextReader sourceReader, PlyHeader header)
            : base(header)
        {
            if (header.Format != PlyFormat.Ascii)
                throw new NotSupportedException($"The {nameof(PlyAsciiReader)} does not support binary data.");
                
            _textReader = sourceReader;
        }

        protected override IEnumerable<T> ReadArrayInternal<T>(PlyArrayProperty expected)
        {
            var countToken = _textReader.ReadToken();
            int count = TryParseInt(countToken);
            for (int i = 0; i < count; ++i)
            {
                var valueToken = _textReader.ReadToken();
                var parsedValue = PlyTypeConverter.ParseStringToNativeType<T>(valueToken, expected.ValueType);
                yield return parsedValue;
            }
        }

        protected override T ReadPropertyInternal<T>(PlyProperty expected)
        {
            var token = _textReader.ReadToken();
            var parsedValue = PlyTypeConverter.ParseStringToNativeType<T>(token, expected.ValueType);
            return parsedValue;
        }

        protected override void SkipPropertyInternal(PlyProperty expected)
        {
            _textReader.ReadToken();
        }

        protected override void SkipPropertyInternal(PlyArrayProperty expected)
        {
            var token = _textReader.ReadToken();
            int count = TryParseInt(token);
            for (int i = 0; i < count; ++i)
                _textReader.ReadToken();
        }

        private int TryParseInt(string value)
        {
            int result;
            if (!int.TryParse(value, out result))
                throw new PlyConverterException(value);

            return result;
        }
    }
}
