using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using game_2.Brain;

namespace game_2
{
    public class GameEngine : GameWindow
    {
        float timer;

        GameObj gameObj;
        GameObj gameObj2;
        GameObj gameObj3;

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

            ///////////////параметры игры
            gameObj = new GameObj("C:\\Users\\Lenovo\\source\\repos\\game_2\\models\\obj_files\\car_sk_exp.obj");
            gameObj2 = new GameObj("C:\\Users\\Lenovo\\source\\repos\\game_2\\models\\obj_files\\final_v01.obj");
            gameObj3 = new GameObj("C:\\Users\\Lenovo\\source\\repos\\game_2\\models\\obj_files\\Steve.obj");

            timer = 0;
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
            GL.Clear(ClearBufferMask.DepthBufferBit);
            timer++;

            gameObj3.Scale(0.03f, 0.03f, 0.03f);     //girl
            gameObj3.Position(-8, -2f, -20f);
            gameObj3.Rotate(0, timer / 50, 0);

            gameObj2.Scale(0.02f, 0.02f, 0.02f);    //man
            gameObj2.Position(0, 0, - 20f);
            gameObj2.Rotate(0, -10 , 0);

            gameObj.Scale(0.5f, 0.5f, 0.5f);    //steve
            gameObj.Position(0, -2.5f + math3d.abs(math3d.sin(timer/500)), -10);
            gameObj.Rotate(0, timer / 20, 0);

            gameObj.Draw();
            gameObj2.Draw();
            gameObj3.Draw();
            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
        }
    }
}
