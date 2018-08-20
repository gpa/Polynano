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
namespace Polynano.Processing.Core.Utils
{
    public struct HalfedgeRef : IMeshElementRef
    {
        public static readonly HalfedgeRef None = new HalfedgeRef(-1);
        public bool IsNone() => Index == -1;

        public int Index { get; set; }

        internal HalfedgeRef(int i)
        {
            Index = i;
        }

        public static bool operator ==(HalfedgeRef a, HalfedgeRef b)
        {
            return a.Index == b.Index;
        }

        public static bool operator !=(HalfedgeRef a, HalfedgeRef b)
        {
            return a.Index != b.Index;
        }

        public override string ToString()
        {
            return Index.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj is HalfedgeRef @ref &&
                   Index == @ref.Index;
        }

        public override int GetHashCode()
        {
            return -2134847229 + Index.GetHashCode();
        }
    }
}
