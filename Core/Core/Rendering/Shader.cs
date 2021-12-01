using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using OpenTK.Graphics.OpenGL;

using Lenia.Core.ErrorHandling;
using Lenia.Core.Common;

namespace Lenia.Core.Rendering
{
    [Serializable]
    public class Shader : IDisposable
    {
        private static readonly Shader defaultShader;

        private readonly int shaderProgram;

        private readonly int vertexShader;
        private readonly int fragmentShader;

        private bool isDisposed = false;
        private bool isCompiled = false;

        public Dictionary<ShaderType, string> shaderNames;

        #region Constructor
        static Shader()
        {
            defaultShader = new Shader();
            GL.ShaderSource(defaultShader.vertexShader, @"
#version 330 core
layout (location = 0) in vec3 aPosition;

uniform mat4 u_MVP;
uniform vec3 u_Position;

void main()
{
    gl_Position = u_MVP * vec4(aPosition + u_Position, 1.0);
}");

            GL.ShaderSource(defaultShader.fragmentShader, @"
#version 330 core
out vec4 FragColor;

void main()
{
    FragColor = vec4(0.5f, 0.8f, 0.7f, 1.0f);
}");

            defaultShader.shaderNames = new Dictionary<ShaderType, string>()
            {
                { ShaderType.VertexShader,   "Default"},
                { ShaderType.FragmentShader, "Default"},
            };

            defaultShader.Compile();
        }
        public Shader()
        {
            vertexShader = GL.CreateShader(ShaderType.VertexShader);
            fragmentShader = GL.CreateShader(ShaderType.FragmentShader);

            shaderProgram = GL.CreateProgram();

            shaderNames = new Dictionary<ShaderType, string>();
        }

        ~Shader()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (isDisposed)
                return;

            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);

            GL.DeleteProgram(shaderProgram);

            isDisposed = true;
        }
        #endregion

        public static Shader Default => defaultShader;
        public int ShaderID => shaderProgram;


        public void Open(string directory, ShaderType shaderType)
        {
            string absoluteDir = Path.Combine(Directories.Shaders, directory);

            if (string.IsNullOrEmpty(absoluteDir))
                throw new ArgumentNullException("directory", "directory is required");

            if (!File.Exists(absoluteDir))
                throw new ArgumentException("directory", "directory does not exist / could not be accessed");


            string source = File.ReadAllText(absoluteDir);
            switch (shaderType)
            {
                case ShaderType.VertexShader:       GL.ShaderSource(vertexShader, source);      break;
                case ShaderType.FragmentShader:     GL.ShaderSource(fragmentShader, source);    break;
            }
            shaderNames[shaderType] = directory;
        }

        public void Compile()
        {
            GL.CompileShader(vertexShader);
            string infoLogVert = GL.GetShaderInfoLog(vertexShader);
            if (!string.IsNullOrEmpty(infoLogVert))
                throw new InvalidOperationException($"Vertex shader compilation failed:\n{infoLogVert}");

            GL.CompileShader(fragmentShader);
            string infoLogFrag = GL.GetShaderInfoLog(fragmentShader);
            if (!string.IsNullOrEmpty(infoLogFrag))
                throw new InvalidOperationException($"Fragment shader compilation failed:\n{infoLogFrag}");

            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);

            GL.LinkProgram(shaderProgram);

            GL.DetachShader(shaderProgram, vertexShader);
            GL.DetachShader(shaderProgram, fragmentShader);

            OpenTKException.ThrowIfErrors();

            isCompiled = true;
        }

        public void UseProgram()
        {
            if (isDisposed)
                throw new ObjectDisposedException("Shader program is already disposed");

            if (!isCompiled)
                throw new InvalidOperationException("Shader must be compiled before use");

            GL.UseProgram(shaderProgram);

            OpenTKException.ThrowIfErrors();
        }
    }
}
