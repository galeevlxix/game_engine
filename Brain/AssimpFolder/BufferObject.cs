using OpenTK.Graphics.OpenGL4;

namespace game_2.Brain.AssimpFolder
{
    public class BufferObject<TDataType> where TDataType : unmanaged
    {
        public int Handle { get; private set; }
        private BufferTarget bufferTarget;

        public unsafe BufferObject(Span<TDataType> data, BufferTarget bufferTarget)
        {
            this.bufferTarget = bufferTarget;

            Handle = GL.GenBuffer();
            Bind();
            GL.BufferData(bufferTarget, data.Length * sizeof(TDataType), data.ToArray(), BufferUsageHint.DynamicDraw);
        }
        public unsafe BufferObject(int amount, BufferTarget bufferTarget)
        {
            this.bufferTarget = bufferTarget;

            Handle = GL.GenBuffer();
            Bind();
            GL.BufferData(bufferTarget, amount * sizeof(TDataType), IntPtr.Zero, BufferUsageHint.DynamicDraw);
        }

        public unsafe void SuberData(Span<TDataType> data)
        {
            Bind();
            GL.BufferSubData(bufferTarget, IntPtr.Zero, data.Length * sizeof(TDataType), data.ToArray());

        }
        public unsafe void SuberData(TDataType[,] data)
        {
            Bind();
            GL.BufferSubData(bufferTarget, IntPtr.Zero, data.Length * sizeof(TDataType), data);

        }
        public void Bind() => GL.BindBuffer(bufferTarget, Handle);
    }
}
