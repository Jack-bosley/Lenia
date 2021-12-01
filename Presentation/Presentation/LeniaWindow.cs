using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

using Lenia.Application;

namespace Lenia.Presentation
{

    public class LeniaWindow : GameWindow
    {
        private bool isCloseRequested = false;

        App app = new App();

        public LeniaWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            RenderThreadStarted += Start;

            RenderFrame += Update;
            UpdateFrame += FixedUpdate;
        }


        #region Event callbacks

        private void Start()
        {
            GL.ClearColor(0.392f, 0.584f, 0.929f, 0.0f);
            GL.Enable(EnableCap.Texture2D);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            app.Start();
        }

        private void Update(FrameEventArgs args)
        {
            if (isCloseRequested)
                return;

            GL.Clear(ClearBufferMask.ColorBufferBit);

            app.Update();

            SwapBuffers();
            GL.Flush();
        }

        private void FixedUpdate(FrameEventArgs args)
        {
            app.FixedUpdate();
        }

        #endregion

        #region Overrides

        protected override void OnResize(ResizeEventArgs args)
        {
            base.OnResize(args);

        }

        protected override void OnClosing(CancelEventArgs args)
        {
            base.OnClosing(args);

            isCloseRequested = true;
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);
        }

        #endregion

    }
}
