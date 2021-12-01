using System;
using System.Collections.Generic;
using System.Text;

using OpenTK.Graphics.OpenGL;

namespace Lenia.Core.Rendering
{
    public interface IVertex
    {
        public int Size { get; }

        internal VertexAttribPointerData[] AttribPointers { get; }

    }
}
