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
    [Serializable]
    public class Texture : IDisposable
    {
        #region Private members
        private Colour[] colours;

        private readonly int textureId;

        private bool isDataBuffered = false;
        private bool isBufferValid = false;
        private bool isDisposed = false;

        private Dictionary<TextureParameterName, int> textureParameters;
        #endregion

        #region Constructor
        public Texture()
        {
            textureId = GL.GenTexture();
            textureParameters = new Dictionary<TextureParameterName, int>()
            {
                { TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest },
                { TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest },
                { TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat },
                { TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder },
            };
        }
        ~Texture()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (isDisposed)
                return;

            GL.DeleteTexture(textureId);

            isDisposed = true;
        }

        #endregion

        #region Properties
        public int TextureID => textureId;

        public int Width { get; private set; }

        public int Height { get; private set; }

        public Colour[] Colours
        {
            get => colours;
            set => colours = value;
        }


        #endregion

        #region Public Methods

        public static int GetUnitNumber(TextureUnit unit) => (int)unit - (int)TextureUnit.Texture0;


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

            BufferData();
            isBufferValid = true;
        }

        public void Bind(TextureUnit unit)
        {
            if (isDisposed)
                throw new ObjectDisposedException("Texture is already disposed");

            if (!isDataBuffered)
                throw new InvalidOperationException("Texture data has not been buffered");

            if (!isBufferValid)
                throw new WarningException("Texture data is likely expired");

            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            OpenTKException.ThrowIfErrors();
        }

        public void LoadTexture(string directory)
        {
            string absoluteDir = Path.Combine(Directories.Assets, directory);

            Bitmap bitmap = Bitmap.FromFile(absoluteDir) as Bitmap;
            Width = bitmap.Width;
            Height = bitmap.Height;

            SysImg.BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), SysImg.ImageLockMode.ReadOnly, SysImg.PixelFormat.Format32bppRgb);

            unsafe
            {
                Colours = new Colour[bitmap.Width * bitmap.Height];
                byte* src = (byte*)data.Scan0;

                int bitStep = data.Stride / Width;
                switch (bitStep)
                {
                    case 4:
                        for (int x = 0; x < Width; x++)
                        {
                            for (int y = 0; y < Height; y++)
                            {
                                colours[x + (Width * y)] = new Colour(src[0], src[1], src[2], src[3]);
                                src += bitStep;
                            }
                        }
                        break;

                    case 3:
                        for (int x = 0; x < Width; x++)
                        {
                            for (int y = 0; y < Height; y++)
                            {
                                colours[x + (Width * y)] = new Colour(src[0], src[1], src[2], 0);
                                src += bitStep;
                            }
                        }
                        break;

                    case 2:
                        for (int x = 0; x < Width; x++)
                        {
                            for (int y = 0; y < Height; y++)
                            {
                                colours[x + (Width * y)] = new Colour(src[0], src[1], 0, 0);
                                src += bitStep;
                            }
                        }
                        break;

                    case 1:
                        for (int x = 0; x < Width; x++)
                        {
                            for (int y = 0; y < Height; y++)
                            {
                                colours[x + (Width * y)] = new Colour(src[0], 0, 0, 0);
                                src += bitStep;
                            }
                        }
                        break;

                }
            }

            bitmap.UnlockBits(data);
            bitmap.Dispose();
        }

        public void SetParameter(TextureParameterName parameterName, TextureWrapMode wrapMode)
        {
            textureParameters[parameterName] = (int)wrapMode;
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            GL.TexParameter(TextureTarget.Texture2D, parameterName, (int)wrapMode);
            OpenTKException.ThrowIfErrors();
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
        public void SetParameter(TextureParameterName parameterName, TextureMagFilter magFilter)
        {
            textureParameters[parameterName] = (int)magFilter;
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            GL.TexParameter(TextureTarget.Texture2D, parameterName, (int)magFilter);
            OpenTKException.ThrowIfErrors();
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
        public void SetParameter(TextureParameterName parameterName, TextureMinFilter minFilter)
        {
            textureParameters[parameterName] = (int)minFilter;
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            GL.TexParameter(TextureTarget.Texture2D, parameterName, (int)minFilter);
            OpenTKException.ThrowIfErrors();
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        #endregion

        #region Private methods

        protected void BufferData()
        {
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, Colours);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            foreach (TextureParameterName parameter in textureParameters.Keys)
                GL.TexParameter(TextureTarget.Texture2D, parameter, textureParameters[parameter]);

            OpenTKException.ThrowIfErrors();

            GL.BindTexture(TextureTarget.Texture2D, 0);

            isDataBuffered = true;
        }
        #endregion

        #region Implicit casts

        public static implicit operator int(Texture texture) => texture.textureId;

        #endregion
    }
}
