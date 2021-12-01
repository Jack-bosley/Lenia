using System;
using System.Collections.Generic;
using System.Text;

namespace Lenia.Core.ErrorHandling
{
    /// <summary>
    /// Soft warning of an error in the system, incident should be logged but not cause any crash
    /// </summary>
    public class OpenTKWarning : Exception
    {


        public OpenTKWarning(string message) : base(message) { }

    }
}
