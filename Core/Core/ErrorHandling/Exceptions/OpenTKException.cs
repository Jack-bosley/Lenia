using System;
using System.Collections.Generic;
using System.Text;

using OpenTK.Graphics.OpenGL;

namespace Lenia.Core.ErrorHandling
{
    public class OpenTKException : Exception
    {
        public readonly ErrorCode ErrorCode = ErrorCode.NoError;

        public OpenTKException(string message) : base(message) { }
        public OpenTKException(ErrorCode errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }


        public static void ThrowIfErrors()
        {
            ErrorCode errorCode;
            if ((errorCode = GL.GetError()) != ErrorCode.NoError)
                throw new OpenTKException(errorCode, $"OpenTK detected an {errorCode} error");
        }
    }
}
