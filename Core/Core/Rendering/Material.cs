using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

using Lenia.Core.ErrorHandling;
using Lenia.Core.Common;

namespace Lenia.Core.Rendering
{
    public class Material : IDisposable
    {
        private static readonly Material defaultMaterial;

        private Shader shader;

        private bool isDisposed = false;

        private List<string> commonUniforms = new List<string>
        {
            "u_MVP",
            "u_Position",
        };

        #region Uniform collections
        private readonly Dictionary<string, int> uniformLocations;

        private readonly Dictionary<string, Texture> textures;
        private readonly Dictionary<string, TextureUnit> textureUnits;

        private readonly Dictionary<string, float>      uniformFloats;
        private readonly Dictionary<string, int>        uniformInts;
        private readonly Dictionary<string, uint>       uniformUints;
        private readonly Dictionary<string, double>     uniformDoubles;
        private readonly Dictionary<string, Half>       uniformHalfs;

        private readonly Dictionary<string, Vector2>    uniformVec2;
        private readonly Dictionary<string, Vector2i>   uniformVec2i;
        private readonly Dictionary<string, Vector2h>   uniformVec2h;

        private readonly Dictionary<string, Vector3>    uniformVec3;
        private readonly Dictionary<string, Vector3i>   uniformVec3i;
        private readonly Dictionary<string, Vector3h>   uniformVec3h;
        
        private readonly Dictionary<string, Vector4>    uniformVec4;
        private readonly Dictionary<string, Vector4i>   uniformVec4i;
        private readonly Dictionary<string, Vector4h>   uniformVec4h;

        private readonly Dictionary<string, Matrix2>    uniformMatrices2;
        private readonly Dictionary<string, Matrix2x3>  uniformMatrices2x3;
        private readonly Dictionary<string, Matrix2x4>  uniformMatrices2x4;

        private readonly Dictionary<string, Matrix3>    uniformMatrices3;
        private readonly Dictionary<string, Matrix3x2>  uniformMatrices3x2;
        private readonly Dictionary<string, Matrix3x4>  uniformMatrices3x4;

        private readonly Dictionary<string, Matrix4>    uniformMatrices4;
        private readonly Dictionary<string, Matrix4x3>  uniformMatrices4x3;
        private readonly Dictionary<string, Matrix4x2>  uniformMatrices4x2;

        // Determine whether new uniforms may be added. If locked values may only be set for already named and located uniforms
        private bool isLocked = false;

        private bool hasTexture = false;
        private bool hasScalar = false;
        private bool hasVector = false;
        private bool hasMatrix = false;
        #endregion

        #region Constructor
        static Material()
        {
            defaultMaterial = new Material()
            {
                shader = Shader.Default,
            };
        }
        public Material()
        {
            textures = new Dictionary<string, Texture>();
            textureUnits = new Dictionary<string, TextureUnit>();
            uniformLocations = new Dictionary<string, int>();


            uniformFloats = new Dictionary<string, float>();
            uniformInts = new Dictionary<string, int>();
            uniformUints = new Dictionary<string, uint>();
            uniformDoubles = new Dictionary<string, double>();
            uniformHalfs = new Dictionary<string, Half>();

            uniformVec2 = new Dictionary<string, Vector2>();
            uniformVec2i = new Dictionary<string, Vector2i>();
            uniformVec2h = new Dictionary<string, Vector2h>();

            uniformVec3 = new Dictionary<string, Vector3>();
            uniformVec3i = new Dictionary<string, Vector3i>();
            uniformVec3h = new Dictionary<string, Vector3h>();

            uniformVec4 = new Dictionary<string, Vector4>();
            uniformVec4i = new Dictionary<string, Vector4i>();
            uniformVec4h = new Dictionary<string, Vector4h>();

            uniformMatrices2 = new Dictionary<string, Matrix2>();
            uniformMatrices2x3 = new Dictionary<string, Matrix2x3>();
            uniformMatrices2x4 = new Dictionary<string, Matrix2x4>();

            uniformMatrices3 = new Dictionary<string, Matrix3>();
            uniformMatrices3x2 = new Dictionary<string, Matrix3x2>();
            uniformMatrices3x4 = new Dictionary<string, Matrix3x4>();

            uniformMatrices4 = new Dictionary<string, Matrix4>();
            uniformMatrices4x3 = new Dictionary<string, Matrix4x3>();
            uniformMatrices4x2 = new Dictionary<string, Matrix4x2>();

        }
        ~Material()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (isDisposed)
                return;

            shader.Dispose();
            foreach (string textureName in textures.Keys)
                textures[textureName].Dispose();

            textures.Clear();
            textureUnits.Clear();

            isDisposed = true;
        }
        #endregion

        #region Proprties

        public static Material Default => defaultMaterial;

        public Shader Shader
        {
            get => shader;
            set => shader = value;
        }
        public Dictionary<string, Texture> Textures => textures;
        public Dictionary<string, TextureUnit> TextureUnits => textureUnits;

        #endregion

        #region Public Methods
        public void Bind()
        {
            Shader.UseProgram();

            if (hasTexture)
            {
                foreach (string textureName in textures.Keys)
                {
                    textures[textureName].Bind(textureUnits[textureName]);
                    GL.Uniform1(uniformLocations[textureName], Texture.GetUnitNumber(textureUnits[textureName]));
                }
            }

            if (hasScalar)
            {
                foreach (string scalarName in uniformFloats.Keys)
                    GL.Uniform1(uniformLocations[scalarName], uniformFloats[scalarName]);

                foreach (string scalarName in uniformInts.Keys)
                    GL.Uniform1(uniformLocations[scalarName], uniformInts[scalarName]);

                foreach (string scalarName in uniformUints.Keys)
                    GL.Uniform1(uniformLocations[scalarName], uniformUints[scalarName]);

                foreach (string scalarName in uniformDoubles.Keys)
                    GL.Uniform1(uniformLocations[scalarName], uniformDoubles[scalarName]);

                foreach (string scalarName in uniformHalfs.Keys)
                    GL.Uniform1(uniformLocations[scalarName], uniformHalfs[scalarName]);
            }

            if (hasVector)
            {
                foreach (string vectorName in uniformVec2.Keys)
                    GL.Uniform2(uniformLocations[vectorName], uniformVec2[vectorName]);

                foreach (string vectorName in uniformVec2i.Keys)
                    GL.Uniform2(uniformLocations[vectorName], uniformVec2i[vectorName]);

                foreach (string vectorName in uniformVec2h.Keys)
                    GL.Uniform2(uniformLocations[vectorName], uniformVec2h[vectorName]);


                foreach (string vectorName in uniformVec3.Keys)
                    GL.Uniform3(uniformLocations[vectorName], uniformVec3[vectorName]);

                foreach (string vectorName in uniformVec3i.Keys)
                    GL.Uniform3(uniformLocations[vectorName], uniformVec3i[vectorName]);

                foreach (string vectorName in uniformVec3h.Keys)
                    GL.Uniform3(uniformLocations[vectorName], uniformVec3h[vectorName]);


                foreach (string vectorName in uniformVec4.Keys)
                    GL.Uniform4(uniformLocations[vectorName], uniformVec4[vectorName]);
                
                foreach (string vectorName in uniformVec4i.Keys)
                    GL.Uniform4(uniformLocations[vectorName], uniformVec4i[vectorName]);

                foreach (string vectorName in uniformVec4h.Keys)
                    GL.Uniform4(uniformLocations[vectorName], uniformVec4h[vectorName]);
            }

            if (hasMatrix)
            {
                foreach (string matrixName in uniformMatrices2.Keys)
                {
                    Matrix2 mat = uniformMatrices2[matrixName];
                    GL.UniformMatrix2(uniformLocations[matrixName], false, ref mat);
                }

                foreach (string matrixName in uniformMatrices2x3.Keys)
                {
                    Matrix2x3 mat = uniformMatrices2x3[matrixName];
                    GL.UniformMatrix2x3(uniformLocations[matrixName], false, ref mat);
                }

                foreach (string matrixName in uniformMatrices2x4.Keys)
                {
                    Matrix2x4 mat = uniformMatrices2x4[matrixName];
                    GL.UniformMatrix2x4(uniformLocations[matrixName], false, ref mat);
                }


                foreach (string matrixName in uniformMatrices3.Keys)
                {
                    Matrix3 mat = uniformMatrices3[matrixName];
                    GL.UniformMatrix3(uniformLocations[matrixName], false, ref mat);
                }

                foreach (string matrixName in uniformMatrices3x2.Keys)
                {
                    Matrix3x2 mat = uniformMatrices3x2[matrixName];
                    GL.UniformMatrix3x2(uniformLocations[matrixName], false, ref mat);
                }

                foreach (string matrixName in uniformMatrices3x4.Keys)
                {
                    Matrix3x4 mat = uniformMatrices3x4[matrixName];
                    GL.UniformMatrix3x4(uniformLocations[matrixName], false, ref mat);
                }


                foreach (string matrixName in uniformMatrices4.Keys)
                {
                    Matrix4 mat = uniformMatrices4[matrixName];
                    GL.UniformMatrix4(uniformLocations[matrixName], false, ref mat);
                }

                foreach (string matrixName in uniformMatrices4x3.Keys)
                {
                    Matrix4x3 mat = uniformMatrices4x3[matrixName];
                    GL.UniformMatrix4x3(uniformLocations[matrixName], false, ref mat);
                }

                foreach (string matrixName in uniformMatrices4x2.Keys)
                {
                    Matrix4x2 mat = uniformMatrices4x2[matrixName];
                    GL.UniformMatrix4x2(uniformLocations[matrixName], false, ref mat);
                }
            }

            OpenTKException.ThrowIfErrors();
        }

        public List<string> GetUniformNamesFromIndices(List<int> uniformLocations)
        {
            Bind();

            List<string> uniformNames = new List<string>(uniformLocations.Count);

            foreach (int uniformLocation in uniformLocations)
                uniformNames.Add(GL.GetActiveUniformName(shader.ShaderID, uniformLocation));

            return uniformNames;
        }

        public void LockUniformNames()
        {
            isLocked = true;
        }

        #region Serialization

        public void LoadMaterial(string saveName)
        {
            byte[] bytes;
            using (FileStream fs = File.OpenRead(Path.Combine(Directories.Materials, $"{saveName}.mtrl")))
            {
                bytes = new byte[fs.Length];
                fs.Read(bytes);
            }

            SetFromBytes(bytes);
        }

        public void LoadMaterialInstance(string saveName)
        {
            byte[] bytes;
            using (FileStream fs = File.OpenRead(Path.Combine(Directories.Materials, $"{saveName}.mtrl")))
            {
                bytes = new byte[fs.Length];
                fs.Read(bytes);
            }

            SetFromBytes(bytes);
        }

        #endregion

        #region Set Uniforms
        public void DeclareUniform(string name)
        {
            int uniformLocation = GL.GetUniformLocation(shader.ShaderID, name);

            if (uniformLocation == -1)
                throw new OpenTKException($"Cannot find texture {name} in material's shader");

            if (isLocked && !uniformLocations.ContainsKey(name))
                throw new OpenTKWarning($"Material is locked to new uniforms and does not recognise {name} as an already known uniform");

            uniformLocations[name] = uniformLocation;
        }


        public void SetTexture(string name, TextureUnit unit, Texture texture)
        {
            int uniformLocation = GL.GetUniformLocation(shader.ShaderID, name);

            if (uniformLocation == -1)
                throw new OpenTKException($"Cannot find texture {name} in material's shader");

            if (isLocked && !uniformLocations.ContainsKey(name))
                throw new OpenTKWarning($"Material is locked to new textures and does not recognise {name} as an already known texture");

            Textures[name] = texture;
            textureUnits[name] = unit;
            uniformLocations[name] = uniformLocation;
            hasTexture = true;

            OpenTKException.ThrowIfErrors();
        }



        public void SetUniform1(string name, float value)
        {
            int uniformLocation = GL.GetUniformLocation(shader.ShaderID, name);

            if (uniformLocation == -1)
                throw new OpenTKException($"Cannot find uniform {name} in material's shader");

            if (isLocked && !uniformLocations.ContainsKey(name))
                throw new OpenTKWarning($"Material is locked to new uniforms and does not recognise {name} as an already known uniform");

            uniformLocations[name] = uniformLocation;
            uniformFloats[name] = value;
            hasScalar = true;

            OpenTKException.ThrowIfErrors();
        }

        public void SetUniform1(string name, int value)
        {
            int uniformLocation = GL.GetUniformLocation(shader.ShaderID, name);

            if (uniformLocation == -1)
                throw new OpenTKException($"Cannot find uniform {name} in material's shader");

            if (isLocked && !uniformLocations.ContainsKey(name))
                throw new OpenTKWarning($"Material is locked to new uniforms and does not recognise {name} as an already known uniform");

            uniformLocations[name] = uniformLocation;
            uniformInts[name] = value;
            hasScalar = true;

            OpenTKException.ThrowIfErrors();
        }

        public void SetUniform1(string name, uint value)
        {
            int uniformLocation = GL.GetUniformLocation(shader.ShaderID, name);

            if (uniformLocation == -1)
                throw new OpenTKException($"Cannot find uniform {name} in material's shader");

            if (isLocked && !uniformLocations.ContainsKey(name))
                throw new OpenTKWarning($"Material is locked to new uniforms and does not recognise {name} as an already known uniform");

            uniformLocations[name] = uniformLocation;
            uniformUints[name] = value;
            hasScalar = true;

            OpenTKException.ThrowIfErrors();
        }

        public void SetUniform1(string name, double value)
        {
            int uniformLocation = GL.GetUniformLocation(shader.ShaderID, name);

            if (uniformLocation == -1)
                throw new OpenTKException($"Cannot find uniform {name} in material's shader");

            if (isLocked && !uniformLocations.ContainsKey(name))
                throw new OpenTKWarning($"Material is locked to new uniforms and does not recognise {name} as an already known uniform");

            uniformLocations[name] = uniformLocation;
            uniformDoubles[name] = value;
            hasScalar = true;

            OpenTKException.ThrowIfErrors();
        }

        public void SetUniform1(string name, Half value)
        {
            int uniformLocation = GL.GetUniformLocation(shader.ShaderID, name);

            if (uniformLocation == -1)
                throw new OpenTKException($"Cannot find uniform {name} in material's shader");

            if (isLocked && !uniformLocations.ContainsKey(name))
                throw new OpenTKWarning($"Material is locked to new uniforms and does not recognise {name} as an already known uniform");

            uniformLocations[name] = uniformLocation;
            uniformHalfs[name] = value;
            hasScalar = true;

            OpenTKException.ThrowIfErrors();
        }


        public void SetUniform2(string name, Vector2 value)
        {
            int uniformLocation = GL.GetUniformLocation(shader.ShaderID, name);

            if (uniformLocation == -1)
                throw new OpenTKException($"Cannot find uniform {name} in material's shader");

            if (isLocked && !uniformLocations.ContainsKey(name))
                throw new OpenTKWarning($"Material is locked to new uniforms and does not recognise {name} as an already known uniform");

            uniformLocations[name] = uniformLocation;
            uniformVec2[name] = value;
            hasVector = true;

            OpenTKException.ThrowIfErrors();
        }

        public void SetUniform2(string name, Vector2i value)
        {
            int uniformLocation = GL.GetUniformLocation(shader.ShaderID, name);

            if (uniformLocation == -1)
                throw new OpenTKException($"Cannot find uniform {name} in material's shader");

            if (isLocked && !uniformLocations.ContainsKey(name))
                throw new OpenTKWarning($"Material is locked to new uniforms and does not recognise {name} as an already known uniform");

            uniformLocations[name] = uniformLocation;
            uniformVec2i[name] = value;
            hasVector = true;

            OpenTKException.ThrowIfErrors();
        }

        public void SetUniform2(string name, Vector2h value)
        {
            int uniformLocation = GL.GetUniformLocation(shader.ShaderID, name);

            if (uniformLocation == -1)
                throw new OpenTKException($"Cannot find uniform {name} in material's shader");

            if (isLocked && !uniformLocations.ContainsKey(name))
                throw new OpenTKWarning($"Material is locked to new uniforms and does not recognise {name} as an already known uniform");

            uniformLocations[name] = uniformLocation;
            uniformVec2h[name] = value;
            hasVector = true;

            OpenTKException.ThrowIfErrors();
        }


        public void SetUniform3(string name, Vector3 value)
        {
            int uniformLocation = GL.GetUniformLocation(shader.ShaderID, name);

            if (uniformLocation == -1)
                throw new OpenTKException($"Cannot find uniform {name} in material's shader");

            if (isLocked && !uniformLocations.ContainsKey(name))
                throw new OpenTKWarning($"Material is locked to new uniforms and does not recognise {name} as an already known uniform");

            uniformLocations[name] = uniformLocation;
            uniformVec3[name] = value;
            hasVector = true;

            OpenTKException.ThrowIfErrors();
        }

        public void SetUniform3(string name, Vector3i value)
        {
            int uniformLocation = GL.GetUniformLocation(shader.ShaderID, name);

            if (uniformLocation == -1)
                throw new OpenTKException($"Cannot find uniform {name} in material's shader");

            if (isLocked && !uniformLocations.ContainsKey(name))
                throw new OpenTKWarning($"Material is locked to new uniforms and does not recognise {name} as an already known uniform");

            uniformLocations[name] = uniformLocation;
            uniformVec3i[name] = value;
            hasVector = true;

            OpenTKException.ThrowIfErrors();
        }

        public void SetUniform3(string name, Vector3h value)
        {
            int uniformLocation = GL.GetUniformLocation(shader.ShaderID, name);

            if (uniformLocation == -1)
                throw new OpenTKException($"Cannot find uniform {name} in material's shader");

            if (isLocked && !uniformLocations.ContainsKey(name))
                throw new OpenTKWarning($"Material is locked to new uniforms and does not recognise {name} as an already known uniform");

            uniformLocations[name] = uniformLocation;
            uniformVec3h[name] = value;
            hasVector = true;

            OpenTKException.ThrowIfErrors();
        }
        

        public void SetUniform4(string name, Vector4 value)
        {
            int uniformLocation = GL.GetUniformLocation(shader.ShaderID, name);

            if (uniformLocation == -1)
                throw new OpenTKException($"Cannot find uniform {name} in material's shader");

            if (isLocked && !uniformLocations.ContainsKey(name))
                throw new OpenTKWarning($"Material is locked to new uniforms and does not recognise {name} as an already known uniform");

            uniformLocations[name] = uniformLocation;
            uniformVec4[name] = value;
            hasVector = true;

            OpenTKException.ThrowIfErrors();
        }

        public void SetUniform4(string name, Vector4i value)
        {
            int uniformLocation = GL.GetUniformLocation(shader.ShaderID, name);

            if (uniformLocation == -1)
                throw new OpenTKException($"Cannot find uniform {name} in material's shader");

            if (isLocked && !uniformLocations.ContainsKey(name))
                throw new OpenTKWarning($"Material is locked to new uniforms and does not recognise {name} as an already known uniform");

            uniformLocations[name] = uniformLocation;
            uniformVec4i[name] = value;
            hasVector = true;

            OpenTKException.ThrowIfErrors();
        }

        public void SetUniform4(string name, Vector4h value)
        {
            int uniformLocation = GL.GetUniformLocation(shader.ShaderID, name);

            if (uniformLocation == -1)
                throw new OpenTKException($"Cannot find uniform {name} in material's shader");

            if (isLocked && !uniformLocations.ContainsKey(name))
                throw new OpenTKWarning($"Material is locked to new uniforms and does not recognise {name} as an already known uniform");

            uniformLocations[name] = uniformLocation;
            uniformVec4h[name] = value;
            hasVector = true;

            OpenTKException.ThrowIfErrors();
        }



        public void SetUniformMat2(string name, Matrix2 value)
        {
            int uniformLocation = GL.GetUniformLocation(shader.ShaderID, name);

            if (uniformLocation == -1)
                throw new OpenTKException($"Cannot find uniform {name} in material's shader");

            if (isLocked && !uniformLocations.ContainsKey(name))
                throw new OpenTKWarning($"Material is locked to new uniforms and does not recognise {name} as an already known uniform");

            uniformLocations[name] = uniformLocation;
            uniformMatrices2[name] = value;
            hasMatrix = true;

            OpenTKException.ThrowIfErrors();
        }

        public void SetUniformMat2x3(string name, Matrix2x3 value)
        {
            int uniformLocation = GL.GetUniformLocation(shader.ShaderID, name);

            if (uniformLocation == -1)
                throw new OpenTKException($"Cannot find uniform {name} in material's shader");

            if (isLocked && !uniformLocations.ContainsKey(name))
                throw new OpenTKWarning($"Material is locked to new uniforms and does not recognise {name} as an already known uniform");

            uniformLocations[name] = uniformLocation;
            uniformMatrices2x3[name] = value;
            hasMatrix = true;

            OpenTKException.ThrowIfErrors();
        }

        public void SetUniformMat2x4(string name, Matrix2x4 value)
        {
            int uniformLocation = GL.GetUniformLocation(shader.ShaderID, name);

            if (uniformLocation == -1)
                throw new OpenTKException($"Cannot find uniform {name} in material's shader");

            if (isLocked && !uniformLocations.ContainsKey(name))
                throw new OpenTKWarning($"Material is locked to new uniforms and does not recognise {name} as an already known uniform");

            uniformLocations[name] = uniformLocation;
            uniformMatrices2x4[name] = value;
            hasMatrix = true;

            OpenTKException.ThrowIfErrors();
        }


        public void SetUniformMat3(string name, Matrix3 value)
        {
            int uniformLocation = GL.GetUniformLocation(shader.ShaderID, name);

            if (uniformLocation == -1)
                throw new OpenTKException($"Cannot find uniform {name} in material's shader");

            if (isLocked && !uniformLocations.ContainsKey(name))
                throw new OpenTKWarning($"Material is locked to new uniforms and does not recognise {name} as an already known uniform");

            uniformLocations[name] = uniformLocation;
            uniformMatrices3[name] = value;
            hasMatrix = true;

            OpenTKException.ThrowIfErrors();
        }

        public void SetUniformMat3x2(string name, Matrix3x2 value)
        {
            int uniformLocation = GL.GetUniformLocation(shader.ShaderID, name);

            if (uniformLocation == -1)
                throw new OpenTKException($"Cannot find uniform {name} in material's shader");

            if (isLocked && !uniformLocations.ContainsKey(name))
                throw new OpenTKWarning($"Material is locked to new uniforms and does not recognise {name} as an already known uniform");

            uniformLocations[name] = uniformLocation;
            uniformMatrices3x2[name] = value;
            hasMatrix = true;

            OpenTKException.ThrowIfErrors();
        }

        public void SetUniformMat3x4(string name, Matrix3x4 value)
        {
            int uniformLocation = GL.GetUniformLocation(shader.ShaderID, name);

            if (uniformLocation == -1)
                throw new OpenTKException($"Cannot find uniform {name} in material's shader");

            if (isLocked && !uniformLocations.ContainsKey(name))
                throw new OpenTKWarning($"Material is locked to new uniforms and does not recognise {name} as an already known uniform");

            uniformLocations[name] = uniformLocation;
            uniformMatrices3x4[name] = value;
            hasMatrix = true;

            OpenTKException.ThrowIfErrors();
        }


        public void SetUniformMat4(string name, Matrix4 value)
        {
            int uniformLocation = GL.GetUniformLocation(shader.ShaderID, name);

            if (uniformLocation == -1)
                throw new OpenTKException($"Cannot find uniform {name} in material's shader");

            if (isLocked && !uniformLocations.ContainsKey(name))
                throw new OpenTKWarning($"Material is locked to new uniforms and does not recognise {name} as an already known uniform");

            uniformLocations[name] = uniformLocation;
            uniformMatrices4[name] = value;
            hasMatrix = true;

            OpenTKException.ThrowIfErrors();
        }

        public void SetUniformMat4x2(string name, Matrix4x2 value)
        {
            int uniformLocation = GL.GetUniformLocation(shader.ShaderID, name);

            if (uniformLocation == -1)
                throw new OpenTKException($"Cannot find texture {name} in material's shader");

            if (isLocked && !uniformLocations.ContainsKey(name))
                throw new OpenTKWarning($"Material is locked to new uniforms and does not recognise {name} as an already known uniform");

            uniformLocations[name] = uniformLocation;
            uniformMatrices4x2[name] = value;
            hasMatrix = true;

            OpenTKException.ThrowIfErrors();
        }

        public void SetUniformMat4x3(string name, Matrix4x3 value)
        {
            int uniformLocation = GL.GetUniformLocation(shader.ShaderID, name);

            if (uniformLocation == -1)
                throw new OpenTKException($"Cannot find texture {name} in material's shader");

            if (isLocked && !uniformLocations.ContainsKey(name))
                throw new OpenTKWarning($"Material is locked to new uniforms and does not recognise {name} as an already known uniform");

            uniformLocations[name] = uniformLocation;
            uniformMatrices4x3[name] = value;
            hasMatrix = true;

            OpenTKException.ThrowIfErrors();
        }

        #endregion
        #endregion

        #region Private Methods

        private byte[] GetBytes(bool includeInstanceValue)
        {
            StringBuilder sb = new StringBuilder();

            // Store the name of the shader to allow easier lookups
            foreach (ShaderType shaderType in shader.shaderNames.Keys)
            {
                string shaderLookup = $"s¦{(int)shaderType}¦{shader.shaderNames[shaderType]}\t";
                sb.Append(shaderLookup);
            }
            sb.Append("c¦\t");

            if (includeInstanceValue)
            {
                // Store uniform names and values
                foreach (string uniformName in uniformLocations.Keys)
                {
                    string uniformLocation = $"u¦{uniformName}¦\t";
                    sb.Append(uniformLocation);
                }
            }
            else
            {
                // Store uniform names
                foreach (string uniformName in uniformLocations.Keys)
                {
                    string uniformLocation = $"u¦{uniformName}\t";
                    sb.Append(uniformLocation);
                }
            }

            // Also store common uniforms
            foreach (string uniformName in commonUniforms)
            {
                string uniformLocation = $"u¦{uniformName}\t";
                sb.Append(uniformLocation);
            }

            return Encoding.Unicode.GetBytes(sb.ToString());
        }

        private void SetFromBytes(byte[] bytes)
        {
            string data = Encoding.Unicode.GetString(bytes);

            shader = new Shader();

            foreach (string line in data.Split("\t"))
            {
                if (!line.Contains("¦"))
                    continue;

                string[] lineSplit = line.Split("¦");
                if (lineSplit.Length == 0)
                    continue;

                switch(lineSplit[0])
                {
                    case "s":
                        // Shader info
                        shader.Open(lineSplit[2], (ShaderType)Convert.ToInt32(lineSplit[1]));
                        break;

                    case "c":
                        // Shader compile command
                        shader.Compile();
                        break;

                    case "u":
                        // Uniform info
                        DeclareUniform(lineSplit[1]);
                        break;
                }
            }

            LockUniformNames();
        }
        #endregion
    }
}
