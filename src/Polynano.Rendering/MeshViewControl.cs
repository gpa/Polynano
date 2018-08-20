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
using OpenTK.Graphics;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Polynano.Rendering
{
    public class MeshViewControl : GLControl
    {
        private Point _lastMousePosition;

        private bool _isViewRotationActive;

        public MouseButtons ViewNavigationTriggerButton { get; set; }

        public IDrawable Mesh { get; set; }

        public RenderTarget RenderTarget { get; private set; }

        public RenderStates RenderStates { get; private set; }

        public event EventHandler OnReady;

        public MeshViewControl(Form parent)
            : base(new GraphicsMode(32, 24, 0, 8), 3, 3, GraphicsContextFlags.Default)
        {
            ViewNavigationTriggerButton = MouseButtons.Left;
            Parent = parent;
        }

        protected override void OnLoad(EventArgs e)
        {
            RenderTarget = new RenderTarget(new Point(Width, Height));
            RenderStates = new RenderStates();
            OnReady?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnResize(EventArgs e)
        {
            if (RenderTarget != null && RenderTarget.Viewport != null)
            {
                RenderTarget.Resize(new Point(Width, Height));
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            RenderTarget.Viewport.Zoom(e.Delta > 0 ? 0.1f * e.Delta + 1.0f : 0.1f * e.Delta - 1.0f);
            Render();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Render();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == ViewNavigationTriggerButton)
            {
                _isViewRotationActive = true;
                _lastMousePosition = new Point(e.X, e.Y);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == ViewNavigationTriggerButton)
            {
                _isViewRotationActive = false;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_lastMousePosition.X == e.X && _lastMousePosition.Y == e.Y)
                return;

            if (_isViewRotationActive)
            {
                RenderTarget.Viewport.Orbit(e.X, e.Y, _lastMousePosition.X, _lastMousePosition.Y);
                _lastMousePosition = new Point(e.X, e.Y);
                Render();
            }
        }

        private void Render()
        {
            if (RenderStates.ShaderProgram == null)
                return;

            RenderStates.ShaderProgram.Bind();
            RenderTarget.Clear();

            if (Mesh != null)
            {
                RenderTarget.Draw(Mesh, RenderStates);
            }

            SwapBuffers();
        }
    }
}
