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
using Polynano.Processing.Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Polynano.Processing.Core.Collections
{
    /// <summary>
    /// A container for faces.
    /// </summary>
    public class FaceCollection : IReadOnlyCollection<IFace>
    {
        /// <summary>
        // Format: 
        // [indexCount] [index] [index] e.g. _3_ 1 2 4 _4_ 5 7 9 4 for a triangle and a square.
        // Count can be also negative indicating that the face is currently deleted.
        /// </summary>
        private readonly List<int> _faceBindings;

        /// <summary>
        /// Face count
        /// </summary>
        public int Count { get; private set; }

        public int CustomCapacityIncreaseOnResize { get; set; } = -1;

        /// <summary>
        /// Enumerate all <see cref="IReadOnlyFace"/> stored in this collection.
        /// </summary>
        public IEnumerator<IFace> Faces
        {
            get
            {
                for (int i = 0; i < _faceBindings.Count; i += Math.Abs(_faceBindings[i]) + 1)
                {
                    if (_faceBindings[i] > 0)
                        yield return new FaceCollectionFace(_faceBindings, i);
                }
            }
        }

        /// <summary>
        /// Create a new face collection.
        /// </summary>
        public FaceCollection()
            : this(0, 0)
        {
        }

        /// <summary>
        /// Create a new face collection
        /// </summary>
        /// <param name="vertexCountPerFaceHint">Defines a hint on how many indices will a typical face have</param>
        /// <param name="faceCountHint">Defines a hint on how many faces will be stored.</param>
        public FaceCollection(int faceCountHint, int vertexCountPerFaceHint)
        {
            var indicesCountHint = Math.Max(1, faceCountHint * (1 + vertexCountPerFaceHint));
            _faceBindings = new List<int>(indicesCountHint);
        }

        /// <summary>
        /// Add a new face to the collection.
        /// </summary>
        /// <param name="vertexIndices">A list of <see cref="VertexRef"/> that make up the face being inserted.</param>
        /// <returns><see cref="FaceRef"/> to be used as an index pointing to the created face.</returns>
        public void AddFace(ICollection<int> vertexIndices)
        {
            if (vertexIndices.Count < 3)
                throw new ArgumentException("A face must have at least 3 vertices.", nameof(vertexIndices));

            if (CustomCapacityIncreaseOnResize != -1 && _faceBindings.Count + vertexIndices.Count > _faceBindings.Capacity)
                _faceBindings.Capacity += CustomCapacityIncreaseOnResize;

            _faceBindings.Add(vertexIndices.Count);
            foreach (var vertexIndex in vertexIndices)
                _faceBindings.Add(vertexIndex);

            Count++;
        }

        public void AddFace(params int[] vertexIndices)
        {
            AddFace((ICollection<int>)vertexIndices);
        }

        public IEnumerator<IFace> GetEnumerator()
            => Faces;

        IEnumerator IEnumerable.GetEnumerator()
            => Faces;
    }
}
