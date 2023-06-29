using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using game_2.Brain;
using OpenTK.Mathematics;

namespace game_2
{
    public class GameEngine : GameWindow
    {
        float timer;

        GameObj gameObj;
        GameObj gameObj1;
        GameObj gameObj2;

        public GameEngine(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }

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
            base.CursorGrabbed = false;
            ///////////////параметры игры
            gameObj = new GameObj("C:\\Users\\Lenovo\\source\\repos\\game_2\\models\\obj_files\\Elf01_posed.obj");
            gameObj1 = new GameObj("C:\\Users\\Lenovo\\source\\repos\\game_2\\models\\SM_HandAxe.ply");
            gameObj2 = new GameObj("C:\\Users\\Lenovo\\source\\repos\\game_2\\models\\fbx_files\\SM_FlatheadScrewdriver.fbx");
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
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);
            timer++;

            gameObj.Scale(0.1f, 0.1f, 0.1f);                //отвертка fbx
            gameObj.Position(-2, math3d.sin(timer/500), -5);
            gameObj.Rotate(0, 0, 0);

            gameObj1.Scale(0.1f, 0.1f, 0.1f);               //девушка obj
            gameObj1.Position(2, -2f, -7);
            gameObj1.Rotate(0, timer / 50, 0);

            gameObj2.Scale(0.1f, 0.1f, 0.1f);               //топор ply
            gameObj2.Position(math3d.sin(timer / 500 + 100) * 4, 0, -10);
            gameObj2.Rotate(0, -180, 0);

            gameObj.Draw();
            gameObj1.Draw();
            gameObj2.Draw();
            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }
    }
}
