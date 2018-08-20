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
    public class Segment
    {
        public Vector3 V1 { get; set; }
        public Vector3 V2 { get; set; }

        public bool Intersects(Ray ray, float errorTolerance, out float shortestDistance, out float shortestDistanceToOrigin)
        {
            shortestDistance = float.PositiveInfinity;
            shortestDistanceToOrigin = float.PositiveInfinity;

            var ab = Vector3.Normalize(V2 - V1);

            // ab x d
            Vector3 abd = Vector3.Cross(ab, ray.Direction);

            // parallel check
            if (abd.LengthSquared() < 0.025)
                return false;

            // ab x (ab x d)
            Vector3 n = Vector3.Cross(ab, abd);

            // ray to plane 
            float t;
            var plane = new Plane(V1, n);
            if (plane.Intersects(ray, out t))
            {
                // projected q 
                Vector3 q = ray.Origin + t * ray.Direction;

                // project q onto ab (s = A distance parameter)
                var s = Vector3.Dot(q - V1, ab);

                Vector3 p;
                // project p, limit it to the segment
                if (s < 0)
                {
                    p = V1;
                }
                else if ((V2 - V1).LengthSquared() < s * s)
                {
                    p = V2;
                }
                else
                {
                    p = V1 + ab * s;
                }

                // get shortest distance from p to our original ray
                var rayDist = ((ray.Origin + ray.Direction * Vector3.Dot(p - ray.Origin, ray.Direction)) - p).LengthSquared();

                // If shorter than margin, they intersect
                if (rayDist < errorTolerance * errorTolerance)
                {
                    shortestDistance = (float)System.Math.Sqrt(rayDist);
                    shortestDistanceToOrigin = t;
                    return true;
                }
            }
            return false;
        }
    }
}
