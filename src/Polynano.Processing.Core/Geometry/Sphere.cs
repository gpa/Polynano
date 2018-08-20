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

namespace Polynano.Processing.Core.Geometry
{
    public class Sphere
    {
        public Vector3 Center { get; set; }
        public float Radius { get; set; }

        public Sphere(Vector3 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public bool Intersects(Ray ray, out float t1, out float t2)
        {
            t1 = t2 = float.PositiveInfinity;

            var oc = ray.Origin - Center;
            var doc = Vector3.Dot(ray.Direction, oc);

            var s = (doc) * (doc) - oc.LengthSquared() + Radius * Radius;

            if (s < 0)
                return false;

            var sq = (float)System.Math.Sqrt(s);

            if (s == 1)
            {
                t1 = -doc + sq;
                return true;
            }

            t1 = -doc + sq;
            t2 = -doc - sq;
            return true;
        }
    }
}
