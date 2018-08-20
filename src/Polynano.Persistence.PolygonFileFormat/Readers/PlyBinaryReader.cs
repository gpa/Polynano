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
using Polynano.Persistence.PolygonFileFormat.Readers.StreamReaders;
using System;
using System.Collections.Generic;

namespace Polynano.Persistence.PolygonFileFormat.Readers
{
    public class PlyBinaryReader : PlyReader
    {
        private readonly IBinaryReader _binaryReader;
        
        private readonly bool _reverseByteOrder;

        public PlyBinaryReader(IBinaryReader binaryReader, PlyHeader header)
            : base(header)
        {
            if (header.Format == PlyFormat.Ascii)
                throw new NotSupportedException($"The {nameof(PlyBinaryReader)} does not support ASCII encoded data.");
            
            _binaryReader = binaryReader;
            bool littleEndian = header.Format == PlyFormat.BinaryLittleEndian;
            _reverseByteOrder = BitConverter.IsLittleEndian != littleEndian;
        }
        
        protected override IEnumerable<T> ReadArrayInternal<T>(PlyArrayProperty expected)
        {
            var countAsBytes = ReadBytesFor(expected.ArraySizeType);

            var count = PlyTypeConverter.ArraySizeInBytesToInt(countAsBytes);
            for (int i = 0; i < count; ++i)
            {
                yield return ReadPropertyInternal<T>(expected);
            }
        }

        protected override T ReadPropertyInternal<T>(PlyProperty expected)
        {
            var value = ReadBytesFor(expected.ValueType);
            return PlyTypeConverter.ParseBytesToNative<T>(value, expected.ValueType);
        }

        protected override void SkipPropertyInternal(PlyProperty expected)
        {
            ReadPropertyInternal<object>(expected);
        }

        protected override void SkipPropertyInternal(PlyArrayProperty expected)
        {
            ReadArrayInternal<object>(expected);
        }

        private byte[] ReadBytesFor(PlyType dataType)
        {
            var typeSize = PlyTypeConverter.GetTypeSize(dataType);
            var readBytes = _binaryReader.ReadBytes(typeSize);
            return ToTargetEndianness(readBytes);
        }

        private byte[] ToTargetEndianness(byte[] bytes)
        {
            if (_reverseByteOrder)
                Array.Reverse(bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
