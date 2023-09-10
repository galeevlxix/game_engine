using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using game_2.Brain;
using OpenTK.Mathematics;
using game_2.Brain.SkyBoxFolder;
using game_2.Brain.AimFolder;
using game_2.Brain.InfoPanelFolder;
using System;

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
        InfoPanel info;
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

            Console.WriteLine("Загрузка камеры...");
            Camera.InitCamera();
            Camera.SetCameraPosition(0, 3, 4);

            Console.WriteLine();

            fps_out_list = new List<double>();

            Console.WriteLine("Загрузка шейдеров...");
            CentralizedShaders.Load();

            Console.WriteLine("Загрузка прицела...");
            aim = new Aim();

            Console.WriteLine("Загрузка шрифта...");
            info = new InfoPanel(InfoPanel.FontType.FullSet);

            Console.WriteLine("Загрузка моделей...");
            Models = new ObjectArray();

            Console.WriteLine("Загрузка скайбокса...");
            skybox = new Skybox();

            Console.WriteLine("Успешное завершение\n");
            loaded = true;
        }

        List<double> fps_out_list;
        double fps_out = 0;

        // Примерно deltaTime = 0.002s
        // Примерно deltaTime = 0,0016s при IsMultiThreaded = true
        // Рендер окна
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            fps_out_list.Add(1 / args.Time);

            if(fps_out_list.Count % 100 == 0)
            {
                fps_out = fps_out_list.Average();
                fps_out_list.Clear();
            }

            InputCallbacks(args.Time);

            Camera.OnRender((float)args.Time);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Models.OnRender((float)args.Time);

            CentralizedShaders.ObjectShader.Use();
            Models.Draw();

            CentralizedShaders.SkyBoxShader.Use();
            skybox.Draw();

            CentralizedShaders.ScreenShader.Use();
            aim.Draw();
            info.PutLineAndDraw(
                "x: " + Math.Round(Camera.Pos.x)   + "\n" +
                "y: " + Math.Round(Camera.Pos.y)   + "\n" +
                "z: " + Math.Round(Camera.Pos.z)   + "\n" +
                "FPS: " + Math.Round(fps_out)      + "\n" +
                DateTime.Now
                );

            SwapBuffers();
            GLFW.PollEvents();
        }

        private void InputCallbacks(double Time)
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
            Camera.OnKeyboard(KeyboardState, (float)Time);
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
            info.OnClear();

            CentralizedShaders.Dispose();

            base.OnClosed();
        }
    }
}
