using System;
using System.Collections.Generic;
using System.Text;

using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace Lenia.Core.Rendering
{
    [Serializable]
    internal struct VertexAttribPointerData
    {
        public readonly int index;
        public readonly int size;
        public readonly VertexAttribPointerType type;
        public readonly bool isNormalized;
        public readonly int stride;
        public readonly int offset;

        internal VertexAttribPointerData(int index, int size, VertexAttribPointerType type, bool isNormalized, int stride, int offset)
        {
            this.index = index;
            this.size = size;
            this.type = type;
            this.isNormalized = isNormalized;
            this.stride = stride;
            this.offset = offset;
        }
    }


    /// <summary>
    /// Vertex containing
    ///     3 floats representing the x, y, z of the vertex
    /// </summary>
    [Serializable]
    public struct PositionVertex : IVertex
    {
        private static readonly int size;
        private static readonly VertexAttribPointerData[] attribPointers;

        public float x, y, z;

        static PositionVertex()
        {
            size = 3 * sizeof(float);

            attribPointers = new VertexAttribPointerData[]
            {
                new VertexAttribPointerData(0, 3, VertexAttribPointerType.Float, false, size, 0),
            };
        }
        public PositionVertex(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public PositionVertex(Vector3 position)
        {
            this.x = position.X;
            this.y = position.Y;
            this.z = position.Z;
        }

        int IVertex.Size => size;
        VertexAttribPointerData[] IVertex.AttribPointers => attribPointers;
    }


    /// <summary>
    /// Vertex containing 
    ///     3 floats representing the x, y, z of the vertex
    ///     4 floats representing the r, g, b, a colour channels 
    /// </summary>
    [Serializable]
    public struct ColouredPositionVertex : IVertex
    {
        private static readonly int size;
        private static readonly VertexAttribPointerData[] attribPointers;

        public float x, y, z;
        public Colour colour;

        static ColouredPositionVertex()
        {
            size = 3 * sizeof(float) + Colour.Size;

            attribPointers = new VertexAttribPointerData[]
            {
                new VertexAttribPointerData(0, 3, VertexAttribPointerType.Float, false, size, 0),
                new VertexAttribPointerData(1, 4, VertexAttribPointerType.UnsignedByte, false, size, 3 * sizeof(float)),
            };
        }
        public ColouredPositionVertex(float x, float y, float z, byte r, byte g, byte b)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.colour = new Colour(r, g, b, 1);
        }
        public ColouredPositionVertex(float x, float y, float z, byte r, byte g, byte b, byte a)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.colour = new Colour(r, g, b, a);
        }

        int IVertex.Size => size;
        VertexAttribPointerData[] IVertex.AttribPointers => attribPointers;
    }


    /// <summary>
    /// Vertex containing 
    ///     3 floats representing the x, y, z of the vertex
    ///     2 floats representing the u, v texture coords 
    /// </summary>
    [Serializable]
    public struct TexturedPositionVertex : IVertex
    {
        private static readonly int size;
        private static readonly VertexAttribPointerData[] attribPointers;

        public float x, y, z;
        public float u, v;

        static TexturedPositionVertex()
        {
            size = 3 * sizeof(float) + 2 * sizeof(float);

            attribPointers = new VertexAttribPointerData[]
            {
                new VertexAttribPointerData(0, 3, VertexAttribPointerType.Float, false, size, 0),
                new VertexAttribPointerData(1, 2, VertexAttribPointerType.Float, false, size, 3 * sizeof(float)),
            };
        }

        public TexturedPositionVertex(float x, float y, float z, float u, float v)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.u = u;
            this.v = v;
        }
        public TexturedPositionVertex(Vector3 position, Vector2 texCoord)
        {
            this.x = position.X;
            this.y = position.Y;
            this.z = position.Z;
            this.u = texCoord.X;
            this.v = texCoord.Y;
        }


        int IVertex.Size => size;
        VertexAttribPointerData[] IVertex.AttribPointers => attribPointers;
    }
}
