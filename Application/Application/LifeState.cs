using System;
using System.Collections.Generic;
using System.Text;

using Lenia.Core.Rendering;

namespace Lenia.Application
{
    public class LifeState
    {
        public Frame frameBuffer;


        public LifeState GetNextState()
        {
            return new LifeState();
        }
    }
}
