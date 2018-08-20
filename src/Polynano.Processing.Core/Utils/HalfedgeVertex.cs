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
using System.Numerics;

namespace Polynano.Processing.Core.Utils
{
    public struct HalfedgeVertex : IDeleted
    {
        public Vector3 Position { get; }

        public HalfedgeRef MemberHalfedge { get; }

        public HalfedgeVertex(Vector3 position, HalfedgeRef halfedge)
        {
            Position = position;
            MemberHalfedge = halfedge;
        }

        internal HalfedgeVertex(HalfedgeVertex halfedgeVertex, HalfedgeRef halfedgeRef)
        {
            Position = halfedgeVertex.Position;
            MemberHalfedge = halfedgeRef;
        }

        internal HalfedgeVertex(HalfedgeVertex halfedgeVertex, Vector3 position)
        {
            Position = position;
            MemberHalfedge = halfedgeVertex.MemberHalfedge;
        }

        public override string ToString()
        {
            return $"he: {MemberHalfedge.Index} x: {Position.X} y: {Position.Y} z: {Position.Z}";
        }

        public bool IsDeleted()
        {
            return MemberHalfedge.IsNone();
        }

        public object GetDeletedClone()
        {
            return new HalfedgeVertex(Position, HalfedgeRef.None);
        }
    }
}