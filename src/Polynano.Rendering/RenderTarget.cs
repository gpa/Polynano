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
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace Polynano.Rendering
{
    public class RenderTarget
    {
        public ViewportWithProjection Viewport { get; private set; }

        public RenderTarget(Point size)
        {
            GL.Enable(EnableCap.DepthTest);
            Viewport = new ViewportWithProjection();
            Resize(size);
        }

        public void Resize(Point newSize)
        {
            Viewport.Resize(newSize);
            GL.Viewport(0, 0, Viewport.Size.X, Viewport.Size.Y);
        }

        public void Draw(IDrawable drawable, RenderStates renderStates)
        {
            renderStates.ShaderProgram.ViewMatrix = Viewport.Translation;
            renderStates.ShaderProgram.ProjectionMatrix = Viewport.Projection;
            drawable.Draw(this, renderStates);
        }

        public void Clear()
        {
            Clear(Color4.Black);
        }

        public void Clear(Color4 color)
        {
            GL.ClearColor(color);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }
    }
}
