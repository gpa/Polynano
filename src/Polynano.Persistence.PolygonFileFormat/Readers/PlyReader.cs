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
using System.Collections.Generic;

namespace Polynano.Persistence.PolygonFileFormat.Readers
{
    /// <summary>
    /// Reader is responsible for the reading of PLY Files.
    /// For more source abstraction generic stream are used.
    /// The class is a state machine because the format has no strict structure.
    /// The class client should read the header first,
    /// determine what data to retrieve and call the appropiate functions in the correct order.
    /// Should the call order be not compatible with the structure definition of the header,
    /// exceptions will be thrown.
    /// </summary>
    public abstract class PlyReader
    {
        private readonly PlyHeaderIterator _iterator;

        public PlyReader(PlyHeader header)
        {
            _iterator = new PlyHeaderIterator(header);
        }

        public T ReadProperty<T>()
        {
            PlyProperty expected = _iterator.CurrentProperty;
            if (expected is PlyArrayProperty)
                throw new PlyReadValueWhenArrayExpectedException(expected);

            _iterator.MoveNext();
            return ReadPropertyInternal<T>(expected);
        }

        public IEnumerable<T> ReadArray<T>()
        {
            PlyProperty expected = _iterator.CurrentProperty;

            if (!(expected is PlyArrayProperty))
                throw new PlyReadValueWhenArrayExpectedException(expected);
            
            _iterator.MoveNext();
            return ReadArrayInternal<T>((PlyArrayProperty)expected);
        }

        public void SkipProperty(int count = 1)
        {
            for (var i = 0; i < count; ++i)
            {
                PlyProperty expected = _iterator.CurrentProperty;
                if (expected is PlyArrayProperty)
                    SkipPropertyInternal((PlyArrayProperty)expected);
                else 
                    SkipPropertyInternal(expected);

                _iterator.MoveNext();
            }
        }

        protected abstract T ReadPropertyInternal<T>(PlyProperty expected);
        protected abstract IEnumerable<T> ReadArrayInternal<T>(PlyArrayProperty expected);
        protected abstract void SkipPropertyInternal(PlyProperty expected);
        protected abstract void SkipPropertyInternal(PlyArrayProperty expected);
    }
}
