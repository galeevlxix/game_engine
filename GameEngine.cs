using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using game_2.Brain;
using OpenTK.Mathematics;
using game_2.MathFolder;

namespace game_2
{
    public class GameEngine : GameWindow
    {
        private bool isMouseDown;
        private bool loaded = false;
        int counter = 0;
        List<double> points = new List<double>();

        private ObjectArray Models;

        private int WindowsWidth;
        private int WindowsHeight;

        private readonly Color4 BackGroundColor;

        public GameEngine(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) 
        {
            BackGroundColor = new Color4(0.102f, 0.102f, 0.153f, 1);
        }

        // Инициализация
        public void Init()
        {
            GLFWBindingsContext binding = new GLFWBindingsContext();
            GL.LoadBindings(binding);
            if (GLFW.Init())
            {
                Console.WriteLine("Успешная инициализация!");
            }
        }

        // Загрузка окна
        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(BackGroundColor);
            base.CursorGrabbed = true;

            ///////////////параметры игры

            Models = new ObjectArray();

            Camera.InitCamera();
            Camera.Pos = new vector3f(0, 3, 4);

            GameTime.Start();
            loaded = true;
        }
        
        // Примерно deltaTime = 0.002s
        // Обновление окна
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            KeyboardState input = KeyboardState;
            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            if (input.IsKeyDown(Keys.P))
            {
                GameTime.PlayOrPause();
            }

            if (isMouseDown)
            {
                mPersProj.ChangeFOV(25);
            }
            else
            {
                mPersProj.ChangeFOV(50);
            }

            if (KeyboardState.IsKeyDown(Keys.LeftAlt))
            {
                GameTime.NextFaster();
            }
            else
            {
                GameTime.Next();
            }

            Camera.OnMouse(-MouseState.Delta.X, -MouseState.Delta.Y);
            Camera.OnKeyboard(KeyboardState);
        }

        // Примерно deltaTime = 0.002s
        // Рендер окна
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            Camera.OnRender();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Models.OnRender();
            Models.Draw();

            SwapBuffers();
            GLFW.PollEvents();
        }

        // Callbacks
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (!isMouseDown && e.Button == MouseButton.Button1) isMouseDown = true;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (isMouseDown && e.Button == MouseButton.Button1) isMouseDown = false;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            WindowsWidth = e.Width;
            WindowsHeight = e.Height;
            if (loaded) mPersProj.ChangeWindowSize(WindowsWidth, WindowsHeight);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            GameTime.End();
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            Models.Clear();
        }
    }
}
