using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace game_2.Brain.ObjectFolder
{
    public class Shader : IDisposable
    {
        protected int Handle;
        protected int MVPID;
        protected int PersProjID;
        protected int CameraPosID;
        protected int CameraRotID;

        public Shader(string vs, string fs)
        {
            Init(vs, fs);
        }

        public void Init(string vs, string fs)
        {
            //дескрипторы шейдеров 
            int VertexShader;
            int FragmentShader;

            //привязка к дескрипторам
            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, vs);

            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, fs);

            //компиляция шейдеров
            CompileShaders(VertexShader, FragmentShader);

            //связываем шейдеры в программу которая может быть запущена на графическом процессоре
            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, VertexShader);
            GL.AttachShader(Handle, FragmentShader);

            GL.LinkProgram(Handle);

            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string infolog = GL.GetProgramInfoLog(Handle);
                Console.WriteLine(infolog);
            }

            MVPID = GL.GetUniformLocation(Handle, "mvp");
            if (MVPID < 0)
            {
                Console.WriteLine("mvp не инициализировалось");
            }

            PersProjID = GL.GetUniformLocation(Handle, "pers");
            if (PersProjID < 0)
            {
                Console.WriteLine("pers не инициализировалось");
            }

            CameraPosID = GL.GetUniformLocation(Handle, "campos");
            if (CameraPosID < 0)
            {
                Console.WriteLine("campos не инициализировалось");
            }

            CameraRotID = GL.GetUniformLocation(Handle, "camrot");
            if (CameraRotID < 0)
            {
                Console.WriteLine("camrot не инициализировалось");
            }

            //очистка вершинных и фрагментных шейдеров
            GL.DetachShader(Handle, VertexShader);
            GL.DetachShader(Handle, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);
        }

        public void CompileShaders(int VertexShader, int FragmentShader)
        {
            GL.CompileShader(VertexShader);

            GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(VertexShader);
                Console.WriteLine(infoLog);
            }

            GL.CompileShader(FragmentShader);

            GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out int success2);
            if (success2 == 0)
            {
                string infoLog = GL.GetShaderInfoLog(FragmentShader);
                Console.WriteLine(infoLog);
            }
        }

        public void setMatrix(Matrix4 m)
        {
            Matrix4 p = mPersProj.PersProjMatrix.ToOpenTK();
            Matrix4 c_pos = Camera.CameraTranslation.ToOpenTK();
            Matrix4 c_rot = Camera.CameraRotation.ToOpenTK();

            GL.UniformMatrix4(MVPID, true, ref m);
            GL.UniformMatrix4(PersProjID, true, ref p);
            GL.UniformMatrix4(CameraPosID, true, ref c_pos);
            GL.UniformMatrix4(CameraRotID, true, ref c_rot);
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteProgram(Handle);

                disposedValue = true;
            }
        }

        ~Shader()
        {
            if (disposedValue == false)
            {
                Console.WriteLine("GPU Resource leak! Did you forget to call Dispose()?");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        //Динамическое извлечение макета шейдера
        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(Handle, attribName);
        }
    }
}
