using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using game_2.Brain;
using OpenTK.Mathematics;
using game_2.MathFolder;
using game_2.Brain.SkyBoxFolder;
using game_2.Brain.AimFolder;

namespace game_2
{
    public class GameEngine : GameWindow
    {
        private bool isMouseDown;
        private bool loaded = false;

        private ObjectArray Models;

        private int WindowsWidth;
        private int WindowsHeight;

        Skybox skybox;

        Aim aim;

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

            Camera.InitCamera();
            Camera.Pos = new vector3f(0, 3, 4);

            fps = new List<double>();

            Models = new ObjectArray();
            skybox = new Skybox();
            aim = new Aim();
            loaded = true;
        }

        List<double> fps;

        // Примерно deltaTime = 0.002s
        // Примерно deltaTime = 0,0016s при IsMultiThreaded = true
        // Рендер окна
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            fps.Add(1 / args.Time);

            InputCallbacks();

            Camera.OnRender((float)args.Time);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                       
            Models.OnRender((float)args.Time);
            Models.Draw();

            skybox.Draw();
            aim.Draw();

            SwapBuffers();
            GLFW.PollEvents();
        }

        private void InputCallbacks()
        {
            KeyboardState input = KeyboardState;
            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            if (isMouseDown)
            {
                mPersProj.ChangeFOV(25);
            }
            else
            {
                mPersProj.ChangeFOV(50);
            }
            Camera.OnMouse(-MouseState.Delta.X, -MouseState.Delta.Y);
            Camera.OnKeyboard(KeyboardState);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {

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
        }

        protected override void OnClosed()
        {
            Models.Clear();
            skybox.OnDelete();
            aim.OnDelete();

            Console.WriteLine((int)fps.Average() + " FPS");

            base.OnClosed();
        }
    }
}
