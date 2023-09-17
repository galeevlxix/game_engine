using game_2.MathFolder;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace game_2.Brain
{
    public class Shader : IDisposable
    {
        protected int Handle;

        private Dictionary<string, int> _uniformLocations;
        private int numberOfUniforms;

        public Shader(string vs, string fs)
        {
            Init(vs, fs);
        }

        private void Init(string vs, string fs)
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

            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out numberOfUniforms);

            _uniformLocations = new Dictionary<string, int>();

            for (int i = 0; i < numberOfUniforms; i++)
            {
                string key = GL.GetActiveUniform(Handle, i, out _, out _);
                int location = GL.GetUniformLocation(Handle, key);
                _uniformLocations.Add(key, location);
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

        public void setMatrices(Matrix4 m)
        {
            Matrix4 p = mPersProj.PersProjMatrix.ToOpenTK();
            Matrix4 c_pos = Camera.CameraTranslation.ToOpenTK();
            Matrix4 c_rot = Camera.CameraRotation.ToOpenTK();

            setMatrices(m, c_pos, c_rot, p);
        }

        public void setMatrices(Matrix4 world, Matrix4 c_pos, Matrix4 c_rot, Matrix4 p)
        {
            setMatrix("world", world);
            setMatrix("pers", p);
            setMatrix("campos", c_pos);
            setMatrix("camrot", c_rot);
        }

        public void setMatrices(Matrix4 world, Matrix4 c_rot, Matrix4 p)
        {
            setMatrix("world", world);
            setMatrix("pers", p);
            setMatrix("camrot", c_rot);
        }

        public void setMatrices(Matrix4 world, Matrix4 p)
        {
            setMatrix("world", world);
            setMatrix("pers", p);
        }

        public void setMatrix(string name, Matrix4 data)
        {
            GL.UniformMatrix4(_uniformLocations[name], true, ref data);
        }

        public void setInt(string name, int data)
        {
            GL.Uniform1(_uniformLocations[name], data);
        }

        public void setFloat(string name, float data)
        {
            GL.Uniform1(_uniformLocations[name], data);
        }
        
        public void setVector3(string name, vector3f data)
        {
            GL.Uniform3(_uniformLocations[name], data.x, data.y, data.z);
        }

        public void Use() => GL.UseProgram(Handle);

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

        public int GetUniformLocation(string uniformName)
        {
            return GL.GetUniformLocation(Handle, uniformName);
        }
    }
}
