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
using System;
using System.Numerics;

namespace Polynano.Processing.Core.Geometry
{
    public class Plane
    {
        public Vector3 PointOnPlane { get; set; }
        public Vector3 Normal { get; set; }

        public Plane(Vector3 pointOnPlane, Vector3 planeNormal)
        {
            PointOnPlane = pointOnPlane;
            Normal = planeNormal;
        }

        public bool Intersects(Ray ray, out float rayIntersectionT)
        {
            float denom = Vector3.Dot(Normal, ray.Direction);

            if (denom < -0.0001f)
            {
                rayIntersectionT = Vector3.Dot(PointOnPlane - ray.Origin, Normal) / denom;

                if (rayIntersectionT >= 0)
                    return true;
            }

            rayIntersectionT = float.PositiveInfinity;
            return false;
        }

        public float GetShortestDistanceToPoint(Vector3 point)
        {
            float dist = Vector3.Dot(point - PointOnPlane, Normal);
            return Math.Abs(dist);
        }
    }
}
