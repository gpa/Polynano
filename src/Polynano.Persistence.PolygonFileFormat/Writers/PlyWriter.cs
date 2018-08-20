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

namespace Polynano.Persistence.PolygonFileFormat.Writers
{
    public abstract class PlyWriter : IDisposable
    {       
        protected readonly PlyHeaderIterator _iterator;

        protected Stream _stream;

        public PlyWriter(Stream targetStream, PlyHeader header)
        {
            if (targetStream == null)
                throw new ArgumentNullException($"Cannot initialize {nameof(PlyWriter)}. Given {nameof(targetStream)} is null.", nameof(targetStream));
            if (header == null)
                throw new ArgumentNullException($"Cannot initialize {nameof(PlyWriter)}. Given {nameof(header)} is null.", nameof(header));
            if (!targetStream.CanWrite)
                throw new ArgumentException($"Cannot initialize {nameof(PlyWriter)}. Given {nameof(targetStream)} is readonly.", nameof(targetStream));
        
            _stream = targetStream;
            _iterator = new PlyHeaderIterator(header);
        }

        public PlyWriter WriteValue<T>(T value) where T : IConvertible, IFormattable
        {    
            EnsureNotClosed();
            EnsureHeaderIsWritten();
            EnsureWriteCallIsValid<T>();

            WriteValueInternal(value);

            _iterator.MoveNext();
            return this;
        }

        public PlyWriter WriteValues<T>(params T[] values) where T : IConvertible, IFormattable
        {
            foreach (var val in values)
                WriteValue(val);
            
            return this;
        }

        public PlyWriter WriteArray<T>(params T[] data) where T : IConvertible, IFormattable
        {
            EnsureNotClosed();
            EnsureHeaderIsWritten();
            EnsureWriteArrayCallIsValid<T>();

            WriteArrayInternal<T>(data);

            _iterator.MoveNext();
            return this;
        }

        protected abstract void WriteHeaderInternal(string header);
        protected abstract void WriteValueInternal<T>(T value) where T : IConvertible, IFormattable;
        protected abstract void WriteArrayInternal<T>(T[] value) where T : IConvertible, IFormattable;

        protected void EnsureNotClosed()
        {
            if (_iterator.IsIterationDone)
                throw new InvalidOperationException("All elements have been already written.");
        }

        private void EnsureWriteArrayCallIsValid<T>()
        {
            var expected = _iterator.CurrentProperty as PlyArrayProperty;
            if (expected == null)
                throw new PlyWriteArrayWhenValueExpectedException(expected);

            if (PlyTypeConverter.ToNative(expected.ValueType) != typeof(T))
                throw new PlyWriteDataTypeMismatchException(expected);
        }

        private void EnsureWriteCallIsValid<T>()
        {
            var expected = _iterator.CurrentProperty;
            if (expected is PlyArrayProperty)
                throw new PlyWriteValueWhenArrayExpectedException(expected);

            if (PlyTypeConverter.ToNative(expected.ValueType) != typeof(T))
                throw new PlyWriteDataTypeMismatchException(expected);
        }

        private void EnsureHeaderIsWritten()
        {
            if (_iterator.IsOnFirstElement && _iterator.IsOnFirstProperty)
            {
                var asciiHeader = PlyHeaderWriter.GetHeader(_iterator.Header);
                WriteHeaderInternal(asciiHeader);
            }
        }

        public virtual void Dispose()
        {
            if (!_iterator.IsIterationDone)
                throw new PlyIterationNotFinishedException();

            _stream = null;
        }

        public void ForceDispose()
        {
            _iterator.JumpToEnd();
            Dispose();
        }
    }
}