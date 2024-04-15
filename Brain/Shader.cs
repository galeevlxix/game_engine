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

        public void setValue(Matrix4 world)
        {
            Matrix4 p = mPersProj.PersProjMatrix.ToOpenTK();
            Matrix4 c_pos = Camera.CameraTranslation.ToOpenTK();
            Matrix4 c_rot = Camera.CameraRotation.ToOpenTK();

            setValue("world", world);
            setValue("wvp", world * c_pos * c_rot * p);
        }

        public void setValue(Matrix4 world, Matrix4 c_pos, Matrix4 c_rot, Matrix4 p)
        {
            setValue("world", world);
            setValue("wvp", world * c_pos * c_rot * p);
        }

        public void setValue(Matrix4 world, Matrix4 c_rot, Matrix4 p)
        {
            setValue("world", world);
            setValue("pers", p);
            setValue("camrot", c_rot);
        }

        public void setValue(Matrix4 world, Matrix4 p)
        {
            setValue("world", world);
            setValue("pers", p);
        }

        public void setValue(string name, Matrix4 data)
        {
            if (_uniformLocations.ContainsKey(name))
            {
                GL.UniformMatrix4(_uniformLocations[name], true, ref data);
            }
        }

        public void setValue(string name, int data)
        {
            //if (_uniformLocations.ContainsKey(name))
                GL.Uniform1(_uniformLocations[name], data);
        }
        
        public void setDiffuseMap()
        {
            GL.Uniform1(_uniformLocations["gMaterial.DiffuseMap"], 0);
        }

        public void setNormalMap()
        {
            GL.Uniform1(_uniformLocations["gMaterial.NormalMap"], 1);
        }

        public void setSpecularMap()
        {
            GL.Uniform1(_uniformLocations["gMaterial.SpecularMap"], 2);
        }

        public void setValue(string name, float data)
        {
            //if (_uniformLocations.ContainsKey(name))
                GL.Uniform1(_uniformLocations[name], data);
        }
        
        public void setValue(string name, vector3f data)
        {
            //if (_uniformLocations.ContainsKey(name))
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

