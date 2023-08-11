using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace game_2.Brain
{
    public class MeshEntry
    {
        public int _VB;
        public int _IB;
        public int _numIndices;
        public int _materialIndex;

        public MeshEntry()
        {
            _VB = -1;
            _IB = -1;
            _numIndices = 0;
            _materialIndex = -1;
        }

        public bool Init(Vertex[] Vertices, int[] Indices)
        {
            _numIndices = Indices.Length;

            _VB = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VB);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertex.Size * Vertices.Length, Vertices, BufferUsageHint.StaticDraw);

            _IB = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _IB);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * Indices.Length, Indices, BufferUsageHint.StaticDraw);

            return true;
        }

        ~MeshEntry()
        {
            if (_VB != -1)
            {
                GL.DeleteBuffer(_VB);
            }

            if (_IB != -1)
            {
                GL.DeleteBuffer(_IB);
            }
        }
    }
}
