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
    public class Triangle
    {
        public Vector3 V1 { get; set; }
        public Vector3 V2 { get; set; }
        public Vector3 V3 { get; set; }
 
        public Vector3 Normal
        {
            get
            {
                var v = V2 - V1;
                var u = V3 - V1;

                return Vector3.Cross(v, u);
            }
        }

        public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            V1 = v1;
            V2 = v2;
            V3 = v3;
        }

        public bool Intersects(Ray ray, float originDistanceLimit, out float outT)
        {
            outT = float.PositiveInfinity;
            var v = V2 - V1;
            var u = V3 - V2;

            var n = Vector3.Normalize(Vector3.Cross(v, u));

            float t;
            var plane = new Plane(V1, n);
            if (plane.Intersects(ray, out t))
            {
                if (t > originDistanceLimit)
                    return false;

                Vector3 proj = ray.Origin + t * ray.Direction;

                var abc = Vector3.Dot(n, Vector3.Cross(v, V3 - V1));
                var pbc = Vector3.Dot(n, Vector3.Cross(V2 - proj, V3 - proj));

                var alpha = pbc / abc;

                if (alpha < 0)
                    return false;

                var pca = Vector3.Dot(n, Vector3.Cross(V3 - proj, V1 - proj));

                var beta = pca / abc;
                var gamma = 1.0f - alpha - beta;

                if (beta < 0 || gamma < 0)
                    return false;

                outT = t;
                return true;
            }
            return false;
        }
    }
}
