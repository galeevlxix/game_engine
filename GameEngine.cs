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
        Camera camera;
        Player player;

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
            base.CursorGrabbed = true;
            GL.ClearColor(0.102f, 0.102f, 0.153f, 1);
            ///////////////параметры игры
            gameObj = new GameObj("C:\\Users\\Lenovo\\source\\repos\\game_2\\models\\SM_HandAxe.ply");
            camera = new Camera();
            player = new Player();
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

            GameTime.Delta = (float)(args.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);
            timer++;

            gameObj.Scale(0.3f, 0.3f, 0.3f);    
            gameObj.Position(0, 0, -20);
            gameObj.Rotate(0, timer / 20, 0);

            
            player.Update(MouseState, KeyboardState);
            camera.setWVM(player.WorldToCamera());
            gameObj.Update(MouseState, KeyboardState);

            gameObj.Draw(camera);
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
