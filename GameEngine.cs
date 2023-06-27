using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using game_2.Brain;
using OpenTK.Mathematics;

namespace game_2
{
    public class GameEngine : GameWindow
    {
        string vertexShader =
            "#version 330                                           \n" +
            "layout (location = 0) in vec3 aPosition;               \n" +
            "out vec4 vertexColor;                                  \n" +
            "uniform mat4 mvp;                                      \n" +
            "void main()                                            \n" +
            "{                                                      \n" +
            "   gl_Position = vec4(aPosition, 1.0) * mvp;           \n" +
            "   vertexColor = vec4(clamp(aPosition, 0.0, 1.0), 1.0);\n" +
            "}";

        string fragmentShader =
            "#version 330                                           \n" +
            "in vec4 vertexColor;                                   \n" +
            "void main() { gl_FragColor = vertexColor; }            \n";

        float[] vertices = {
            0.5f, -0.5f, -0.5f,
              0.5f, -0.5f,  0.5f,
             -0.5f, -0.5f,  0.5f,
             -0.5f, -0.5f, -0.5f,
              0.5f,  0.5f, -0.5f,
              0.5f,  0.5f,  0.5f,
             -0.5f,  0.5f,  0.5f,
             -0.5f,  0.5f, -0.5f
        };

        int[] indices = {  // note that we start from 0!
            0,1,2, // передняя сторона
                2,3,0,

                6,5,4, // задняя сторона
                4,7,6,

                4,0,3, // левый бок
                3,7,4,

                1,5,6, // правый бок
                6,2,1,

                4,5,1, // вверх
                1,0,4,

                3,2,6, // низ
                6,7,3,
        };

        int VBO;
        int VAO;
        int IBO;
        Shader shader;
        Pipeline p;
        double timer;

        public GameEngine(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {

        }

        public void Init()
        {
            GLFWBindingsContext binding = new GLFWBindingsContext();
            GL.LoadBindings(binding);
            if (GLFW.Init())
            {
                Console.WriteLine("Успешная инициализация!");
            }
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.0f, 0.0f, 0.5f, 0.1f);

            VBO = GL.GenBuffer();             
            VAO = GL.GenVertexArray();
            IBO = GL.GenBuffer();

            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);

            shader = new Shader(vertexShader, fragmentShader);

            ///////////////Отрисовка
            p = new Pipeline();
            timer = 0;

            p.PersProj(45.0f, 1920, 1080, 0.1f, 100.0f);

        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            

            KeyboardState input = KeyboardState;
            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            } 
            else if (input.IsKeyDown(Keys.W))
            {

            }
            else if (input.IsKeyDown(Keys.S))
            {

            }
            else if (input.IsKeyDown(Keys.A))
            {

            }
            else if (input.IsKeyDown(Keys.D))
            {

            }


        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            timer++;
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            Matrix4 rotation = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(math3d.sin((float)timer / 500)*50));
            Matrix4 scale = Matrix4.CreateScale(0.5f, 0.5f, 0.5f);
            Matrix4 pos = Matrix4.CreateTranslation(1, math3d.abs(math3d.sin((float)timer/500)/2) , -2);
            Matrix4 proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), 1920 / 1080, 0.1f, 100.0f);
            Matrix4 trans = rotation * scale * pos * proj;

            p.Scale(0.5f, 0.5f, 0.5f);
            p.Position(0, math3d.abs(math3d.sin((float)timer / 500) / 2), -3);
            p.Rotate(math3d.sin((float)timer / 500) * 50, (float)timer / 50, 0);

            shader.setMatrix(p.getMVP());

            shader.Use();
            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
        }
    }
}
