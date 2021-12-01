using System;
using System.Collections.Generic;
using System.Text;

namespace Lenia.Core.Rendering
{
    public interface IRenderable
    {
        public Material Material { get; set; }

        public void Draw();
    }
}
