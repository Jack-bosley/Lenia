using System;
using System.IO;
using System.Drawing;
using SysImg = System.Drawing.Imaging;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

using Lenia.Core.ErrorHandling;
using Lenia.Core.Common;


namespace Lenia.Core.Rendering
{
    public class Frame
    {

        #region Private members
        private readonly int framebufferId;

        private bool isBufferValid = false;
        private bool isDisposed = false;

        private Texture renderTarget;
        #endregion

        #region Constructor
        public Frame()
        {
            framebufferId = GL.GenFramebuffer();
            renderTarget = new Texture();
        }
        ~Frame()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (isDisposed)
                return;

            GL.DeleteFramebuffer(framebufferId);

            isDisposed = true;
        }

        #endregion

        #region Properties
        public int TextureID => framebufferId;

        public int Width { get; private set; }

        public int Height { get; private set; }

        #endregion

        #region Public Methods

        public void SetSize(int width, int height)
        {
            Width = width;
            Height = height;
            renderTarget.LoadTexture(width, height);
        }

        /// <summary>
        /// Manually invalidate the buffer - Requires an update call to use again
        /// </summary>
        public void InvalidateBuffers()
        {
            isBufferValid = false;
        }

        /// <summary>
        /// Pushes Vertices and Indices to the GPU
        /// </summary>
        public void Update()
        {
            if (isBufferValid)
                return;

            renderTarget.Update(true);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, renderTarget, 0);

            isBufferValid = true;
        }

        public void Bind()
        {
            if (isDisposed)
                throw new ObjectDisposedException("Texture is already disposed");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, this);
            OpenTKException.ThrowIfErrors();
        }

        #endregion

        #region Private methods

        protected void BufferData()
        {

        }
        #endregion

        #region Implicit casts

        public static implicit operator int(Frame frame) => frame.framebufferId;

        #endregion
    }
}
