using OpenTK.Graphics.OpenGL4;

namespace game_2.Brain.NewAssimpFolder
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
        public void Bind() => GL.BindBuffer(bufferTarget, Handle);

        public void Dispose() => GL.DeleteBuffer(Handle);
    }
}
