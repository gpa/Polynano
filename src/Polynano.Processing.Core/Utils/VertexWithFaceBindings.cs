﻿/*
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
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Polynano.Processing.Core.Utils
{
    public struct VertexWithFaceBindings : IDeleted
    {
        public Vector3 Position { get; set; }

        public Vector3 Normal { get; set; }

        public List<FaceRef> ConnectedFaces { get; set; }

        private bool _isDeleted;

        public VertexWithFaceBindings(Vector3 position)
        {
            Position = position;
            ConnectedFaces = new List<FaceRef>();
            Normal = Vector3.Zero;
            _isDeleted = false;
        }

        public VertexWithFaceBindings(VertexWithFaceBindings other, Vector3 position, bool isDeleted = false)
        {
            Position = position;
            Normal = other.Normal;
            ConnectedFaces = other.ConnectedFaces;
            _isDeleted = isDeleted;
        }

        public VertexWithFaceBindings Clone()
        {
            var vertex =  new VertexWithFaceBindings(Position)
            {
                Normal = Normal,
                ConnectedFaces = ConnectedFaces.ToList()
            };

            vertex.SetDeleted(_isDeleted);
            return vertex;
        }

        public bool IsDeleted()
        {
            return _isDeleted;
        }

        public void SetDeleted(bool isDeleted)
        {
            _isDeleted = isDeleted;
        }

        public object GetDeletedClone()
        {
            return new VertexWithFaceBindings(this, Position, true);
        }
    }
}
