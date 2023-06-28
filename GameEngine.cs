using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using game_2.Brain;

namespace game_2
{
    public class GameEngine : GameWindow
    {
        double timer;

        GameObj gameObj;

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

            GL.ClearColor(0.102f, 0.102f, 0.153f, 1);

            BufferSettings();

            ///////////////параметры игры
            gameObj = new GameObj();

            vector3 c_pos = new vector3(0, 0, -3);
            vector3 c_target = new vector3(0, 0, 1);
            vector3 c_up = new vector3(0, 1, 0);

            timer = 0;

        }

        private void BufferSettings()
        {

            base.CursorGrabbed = true;
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
            else if (input.IsKeyDown(Keys.Space))
            {
            }
            else if (input.IsKeyDown(Keys.LeftShift))
            {
            }

        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit);
            timer++;

            /*p.Scale(0.5f, 0.5f, 0.5f);
            p.Position(0, math3d.abs(math3d.sin((float)timer / 500) / 2) - 0.25f, -2);
            p.Rotate(math3d.sin((float)timer / 500) * 50, (float)timer / 50, 0);*/

            gameObj.Scale(0.5f, 0.5f, 0.5f);
            gameObj.Position(0, math3d.abs(math3d.sin((float)timer / 500) / 2) - 0.25f, -2);
            gameObj.Rotate(math3d.sin((float)timer / 500) * 50, (float)timer / 50, 0);

            gameObj.Draw();

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
        }
    }
}
