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
using OpenTK;
using System;
using System.Drawing;

namespace Polynano.Rendering
{
    public class Viewport
    {
        public Point Size { get; protected set; }

        public Vector3 Origin { get; set; } = new Vector3(0, 0, -3);

        public Matrix4 Translation { get; protected set; }

        public Viewport()
        {
            Translation = Matrix4.CreateTranslation(Origin);
        }

        public virtual void Orbit(int lastx, int lasty, int currentx, int currenty)
        {
            var va = GetVectorToSphere(currentx, currenty);
            var vb = GetVectorToSphere(lastx, lasty);

            float angle = (float)Math.Acos(Math.Min(1.0f, Vector3.Dot(va, vb)));
            var axisInCameraCoord = Vector3.Cross(va, vb);

            var translation = Matrix4.CreateTranslation(Origin);
            Translation = ((Translation * translation.Inverted()) * Matrix4.CreateFromAxisAngle(axisInCameraCoord, angle)) * translation;
        }

        public virtual void Zoom(float factor)
        {
            var translation = Matrix4.CreateTranslation(Origin);
            Translation = (Translation * translation.Inverted()) * Matrix4.CreateScale(factor > 0 ? 1.1f : 0.9f) * translation;
        }

        public virtual void Resize(Point size)
        {
            Size = size;
        }

        private Vector3 GetVectorToSphere(int x, int y)
        {
            float vx = 1.0f * x / Size.X * 2f - 1.0f;
            float vy = -(1.0f * y / Size.Y * 2f - 1.0f);
            float m = vx * vx + vy * vy;

            if (m <= 1 * 1)
                return new Vector3(vx, vy, (float)Math.Sqrt(1 * 1 - m));
            else
                return new Vector3(vx, vy, 0f).Normalized();
        }
    }
}
