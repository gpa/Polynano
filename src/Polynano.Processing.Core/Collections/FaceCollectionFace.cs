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
using Polynano.Core.Collections;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Polynano.Processing.Core.Collections
{
    public class FaceCollectionFace : IFace
    {
        /// <summary>
        /// Reference to the faceCollection faceBindings, 
        /// contains all indices from all faces in the collection.
        /// </summary>
        private readonly List<int> _faceBindings;

        /// <summary>
        /// The index this face starts at in the faceBindings collection.
        /// </summary>
        private readonly int _beginIndex;

        public int Count
        {
            get
            {
                return _faceBindings[_beginIndex];
            }
        }

        public int this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return _faceBindings[_beginIndex + 1 + index];
            }
        }

        public FaceCollectionFace(List<int> faceBindings, int beginIndex)
        {
            _faceBindings = faceBindings;
            _beginIndex = beginIndex;
        }

        public IEnumerator<int> GetEnumerator()
        {
            for (var i = 0; i < Count; ++i)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
