using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using game_2.Brain;
using OpenTK.Mathematics;
using game_2.Brain.SkyBoxFolder;
using game_2.Brain.AimFolder;
using game_2.Brain.InfoPanelFolder;
using game_2.Brain.Lights;
using game_2.Brain.MonochromeObjectFolder;
using game_2.MathFolder;

namespace game_2
{
    public class GameEngine : GameWindow
    {
        private bool isMouseDown;
        private bool loaded = false;

        private int WindowsWidth;
        private int WindowsHeight;

        private ObjectArray Models;
        private Skybox skybox;
        private InfoPanel info;
        private Aim aim;
        private MonochromeObject monochrome;

        LightingTechnique lightConfig;

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
                Console.WriteLine("Успешная инициализация GLFW!");
            }
        }

        // Загрузка окна
        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(BackGroundColor);
            GL.Enable(EnableCap.DepthTest);

            base.CursorGrabbed = true;

            ///////////////параметры игры

            Console.WriteLine("Загрузка камеры...");
            Camera.InitCamera();
            Camera.SetCameraPosition(0, 3, 4);

            fps_out_list = new List<double>();

            Console.WriteLine("Загрузка шейдеров...");
            CentralizedShaders.Load();

            Console.WriteLine("Загрузка прицела...");
            aim = new Aim();

            Console.WriteLine("Загрузка моделей...");
            Models = new ObjectArray();

            Console.WriteLine("Загрузка скайбокса...");
            skybox = new Skybox();

            Console.WriteLine("Загрузка шрифта...");
            info = new InfoPanel(InfoPanel.FontType.EnglishWithNumbers);

            lightConfig = new LightingTechnique();

            BaseLight baseLight = new BaseLight();
            baseLight.Color = new vector3f(1, 1, 1);
            baseLight.AmbientIntensity = 0.3f;

            CentralizedShaders.ObjectShader.Use();
            lightConfig.SetBaseLight(baseLight);

            monochrome = new MonochromeObject(new vector3f(1, 1, 1), new vector3f(1, 1, 1));
            monochrome.pipeline.SetScale(0.5f);
            monochrome.pipeline.SetPosition(0, 4, 5);



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

            // расчет среднего FPS
            fps_out_list.Add(1 / args.Time);
            if (fps_out_list.Count % 100 == 0)
            {
                fps_out = fps_out_list.Average();
                fps_out_list.Clear();
            }

            // управление камерой
            InputCallbacks(args.Time);
            Camera.OnRender((float)args.Time);

            GL.Uniform3(CentralizedShaders.ObjectShader.GetUniformLocation("cTarget"), Camera.Target.x, Camera.Target.y, Camera.Target.z);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Models.OnRender((float)args.Time);

            CentralizedShaders.ObjectShader.Use();
            Models.Draw();

            CentralizedShaders.SkyBoxShader.Use();
            skybox.Draw();

            CentralizedShaders.ScreenShader.Use();
            aim.Draw();
            info.PutLineAndDraw( Math.Round(fps_out) + "fps" );

            CentralizedShaders.MonochromeShader.Use();
            monochrome.Draw();

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
