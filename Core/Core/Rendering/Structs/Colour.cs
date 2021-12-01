using System;
using System.Collections.Generic;
using System.Text;

namespace Lenia.Core.Rendering
{
    [Serializable]
    public struct Colour
    {
        public byte r, g, b, a;

        public Colour(byte r, byte g, byte b, byte a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        /// <summary>
        /// Size of Colour in bytes
        /// </summary>
        public static int Size => 4;

        /// <summary>
        /// RGBA = (255,   0,   0, 255)
        /// </summary>
        public static readonly Colour Red       = new Colour(255,   0,   0, 255);

        /// <summary>
        /// RGBA = (255, 255,   0, 255)
        /// </summary>
        public static readonly Colour Yellow    = new Colour(255, 255,   0, 255);

        /// <summary>
        /// RGBA = (  0, 255,   0, 255)
        /// </summary>
        public static readonly Colour Green     = new Colour(  0, 255,   0, 255);

        /// <summary>
        /// RGBA = (  0,   0, 255, 255)
        /// </summary>
        public static readonly Colour Blue      = new Colour(  0,   0, 255, 255);

        /// <summary>
        /// RGBA = (  0,   0, 128, 255)
        /// </summary>
        public static readonly Colour DarkBlue  = new Colour(  0,   0, 128, 255);
    }
}
