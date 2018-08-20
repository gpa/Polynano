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
using System;
using System.IO;
using System.Text;
using SystemBinaryWriter = System.IO.BinaryWriter;

namespace Polynano.Persistence.PolygonFileFormat.Writers
{
    public class PlyBinaryWriter : PlyWriter 
    {
        private SystemBinaryWriter _writer;

        private readonly bool _reverseByteOrder;

        public PlyBinaryWriter(Stream targetStream, PlyHeader header)
            : base(targetStream, header)
        {
            if (header.Format == PlyFormat.Ascii)
                throw new NotSupportedException($"The {nameof(PlyBinaryWriter)} does not support ascii format.");

            _writer = new SystemBinaryWriter(targetStream, Encoding.BigEndianUnicode, leaveOpen: true);
            var isLittleEndian = header.Format == PlyFormat.BinaryLittleEndian;
            _reverseByteOrder = isLittleEndian != BitConverter.IsLittleEndian;
        }

        protected override void WriteHeaderInternal(string header)
        {
            var asciiHeader = Encoding.ASCII.GetBytes(header);
            _writer.Write(asciiHeader);
        }

        protected override void WriteValueInternal<T>(T value)
        {
            var valueAsBytes = GetBytes(value);
            _writer.Write(valueAsBytes);
        }

        protected override void WriteArrayInternal<T>(params T[] arrayValues)
        {
            var arrayCountPrefixAsBytes = GetBytesForArrayPrefix(arrayValues.Length);
            _writer.Write(arrayCountPrefixAsBytes);

            foreach (T value in arrayValues)
                WriteValueInternal(value);
        }

        private byte[] GetBytes<T>(T value) where T: IConvertible
        {
            var valueAsBytes = PlyTypeConverter.ToBytes(value, _iterator.CurrentProperty.ValueType);
            return ToTargetEndianness(valueAsBytes);
        }

        private byte[] GetBytesForArrayPrefix(int arrayLength)
        {
            var arrayProperty = (PlyArrayProperty)_iterator.CurrentProperty;
            var type = arrayProperty.ArraySizeType;

            EnsureValueFitsType(arrayLength, type);
            byte[] countAsBytes = PlyTypeConverter.ToBytes(arrayLength, type);

            return ToTargetEndianness(countAsBytes);
        }

        private void EnsureValueFitsType(int value, PlyType type)
        {
            int max = int.MaxValue;
            if (type == PlyType.Char)
                max = char.MaxValue;
            else if (type == PlyType.Uchar)
                max = byte.MaxValue;
            else if (type == PlyType.Short)
                max = short.MaxValue;
            else if (type == PlyType.Ushort)
                max = ushort.MaxValue;

            if (value > max)
                throw new PlyValueDoesNotFitTypeException(value, type);
        }

        private byte[] ToTargetEndianness(byte[] bytes)
        {
            if (_reverseByteOrder)
                Array.Reverse(bytes, 0, bytes.Length);
            return bytes;
        }

        public override void Dispose()
        {
            _writer.Dispose();
            base.Dispose();
        }
    }
}
