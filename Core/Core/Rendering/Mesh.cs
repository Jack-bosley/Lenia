using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

using OpenTK.Graphics.OpenGL;

using Lenia.Core.ErrorHandling;

namespace Lenia.Core.Rendering
{
    [Serializable]
    public class Mesh<Vert> : Mesh, IDisposable
        where Vert : struct, IVertex
    {
        #region Private members
        private Vert[] vertices;
        private uint[] indices;

        private int vertexBufferObject;
        private int indexBufferObject;
        private int vertexArrayObject;

        private bool isDataBuffered = false;
        private bool isBufferValid = false;
        private bool isDisposed = false;
        #endregion

        #region Constructor / Destructor
        public Mesh()
        {
            vertexBufferObject = GL.GenBuffer();
            indexBufferObject = GL.GenBuffer();
            vertexArrayObject = GL.GenVertexArray();

        }

        ~Mesh()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (isDisposed)
                return;

            GL.DeleteBuffer(vertexBufferObject);
            GL.DeleteBuffer(indexBufferObject);
            GL.DeleteVertexArray(vertexArrayObject);

            isDisposed = true;

            base.Dispose();
        }
        #endregion

        #region Properties
        public PrimitiveType PrimitiveType => PrimitiveType.Triangles;
        public BeginMode BeginMode => BeginMode.Triangles;
        public DrawElementsType ElementType => DrawElementsType.UnsignedInt;
        public int Count => Indices.Length;

        /// <summary>
        /// Vertices used in the mesh - Set invalidates the buffer and required an Update call
        /// </summary>
        public Vert[] Vertices
        {
            get => vertices;
            set
            {
                vertices = value;

                // Buffers are no longer valid as the desired vertices have changed
                InvalidateBuffers();
            }
        }
        internal VertexAttribPointerData[] VertexAttributePointerData => new Vert().AttribPointers;
        private int VertSize => new Vert().Size; 
        private int VerticesSize => VertSize * vertices.Length;

        /// <summary>
        /// Indices used in the mesh - Set invalidates the buffer and required an Update call
        /// </summary>
        public uint[] Indices
        {
            get => indices;
            set
            {
                if (value != null && value.Length % 3 != 0)
                    throw new ArgumentException("Indices", $"Invalid number of indices supplied; {Indices.Length} is not a multiple of 3");

                indices = value;

                // Buffers are no longer valid as the desired indices have changed
                InvalidateBuffers();
            }
        }
        private int IndexSize => sizeof(uint);
        private int IndicesSize => IndexSize * Indices.Length;
        #endregion

        #region Public Methods
        /// <summary>
        /// Manually invalidate the buffer - Requires an update call to use again
        /// </summary>
        public void InvalidateBuffers()
        {
            isBufferValid = false;
        }

        /// <summary>
        /// Pushes Vertices and Indices to the GPU
        /// </summary>
        public void Update()
        {
            if (vertices == null && vertices.Length == 0)
                throw new ArgumentNullException("Vertices", "Vertices null or empty");

            if (indices == null && indices.Length == 0)
                throw new ArgumentNullException("Indices", "Indices null or empty");


            BufferData();
            isBufferValid = true;
        }

        public override void Draw()
        {
            if (isDisposed)
                throw new ObjectDisposedException("Shader program is already disposed");

            if (!isDataBuffered)
                throw new InvalidOperationException("Mesh data has not been buffered");

            if (!isBufferValid)
                throw new WarningException("Mesh data is likely expired");


            GL.BindVertexArray(vertexArrayObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferObject);
            GL.DrawElements(PrimitiveType, Count, ElementType, 0);

            OpenTKException.ThrowIfErrors();
        }
        #endregion

        #region Private methods

        protected void BufferData()
        {
            // Set up VAO and VBO
            GL.BindVertexArray(vertexArrayObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);

            GL.BufferData(BufferTarget.ArrayBuffer, VerticesSize, vertices, BufferUsageHint.StaticDraw);
            foreach (VertexAttribPointerData attribPointerData in VertexAttributePointerData)
            {
                GL.EnableVertexAttribArray(attribPointerData.index);

                GL.VertexAttribPointer(attribPointerData.index, attribPointerData.size, 
                    attribPointerData.type, attribPointerData.isNormalized,
                    attribPointerData.stride, attribPointerData.offset);

            }

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            OpenTKException.ThrowIfErrors();

            // Set up IBO
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, IndicesSize, indices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            OpenTKException.ThrowIfErrors();

            isDataBuffered = true;
        }
        #endregion
    }

    [Serializable]
    public abstract class Mesh : IDisposable
    {
        public abstract void Draw();

        public Mesh() { }
        ~Mesh() 
        { 
            Dispose(); 
        }
        public virtual void Dispose() { }
    }

}
