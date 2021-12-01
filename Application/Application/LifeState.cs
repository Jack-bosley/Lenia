using System;
using System.Collections.Generic;
using System.Text;

using Lenia.Core.Rendering;

namespace Lenia.Application
{
    public class LifeState
    {
        public Texture lifeTexture;


        public LifeState GetNextState()
        {
            return new LifeState();
        }
    }
}
