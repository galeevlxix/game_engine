using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using game_2.Brain;
using OpenTK.Mathematics;
using game_2.Brain.SkyBoxFolder;
using game_2.Brain.AimFolder;
using game_2.Brain.Lights;
using game_2.Brain.InfoPanelFolder;
using game_2.Brain.Compiler;
using game_2.Brain.ObjectFolder;
using game_2.Brain.NewAssimpFolder;
using game_2.Brain.Lights.LightStructures;

namespace game_2
{
    public class GameEngine : GameWindow
    {
        private bool isMouseDown;
        private bool isLoaded = false;
        private bool isFocused = true; 

        private int WindowsWidth;
        private int WindowsHeight;

        //private ObjectArray Models;
        //private AssimpObjectArray AssimpModels;
        private Skybox skybox;
        private InfoPanel info;
        private Aim aim;
        private ObjectArray models;

        private readonly Color4 BackGroundColor;

        public GameEngine(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) 
        {
            BackGroundColor = new Color4(0.102f, 0.102f, 0.153f, 1);
        }

        // Инициализация
        public async void Init()
        {
            GLFWBindingsContext binding = new GLFWBindingsContext();
            GL.LoadBindings(binding);
            if (GLFW.Init())
            {
                Console.WriteLine("Успешная инициализация GLFW!");
            }
        }

        // Загрузка окна
        protected override async void OnLoad()
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
            CentralizedShaders.ScreenShader.Use();
            aim = new Aim();

            Console.WriteLine("Загрузка шрифта...");
            //info = new InfoPanel(InfoPanel.FontType.EnglishWithNumbersAndPunctuation);

            Console.WriteLine("Загрузка моделей (assimp)...");
            CentralizedShaders.AssimpShader.Use();
            models = new ObjectArray();            
            CentralizedShaders.AssimpShader.setDiffuseMap();
            CentralizedShaders.AssimpShader.setNormalMap();

            Console.WriteLine("Загрузка скайбокса...");
            CentralizedShaders.SkyBoxShader.Use();
            skybox = new Skybox();

            Console.WriteLine("Загрузка света...");
            LightningManager.Init();

            Console.WriteLine("Успешное завершение\n");
            isLoaded = true;

            await Task.Run(() => ConsoleCompiler.Run());
        }

        List<double> fps_out_list;
        double fps_out = 0;

        // Рендер окна
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            //if (!isFocused) return;
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

            GL.Uniform3(CentralizedShaders.AssimpShader.GetUniformLocation("cTarget"), Camera.Target.x, Camera.Target.y, Camera.Target.z);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            CentralizedShaders.AssimpShader.Use();
            models.Draw();          

            CentralizedShaders.SkyBoxShader.Use();
            skybox.Draw();

            CentralizedShaders.ScreenShader.Use();
            aim.Draw();

            CentralizedShaders.MonochromeShader.Use();

            float dt = (float)args.Time * 2;

            LightningManager.Render(dt);

            ConsoleCompiler.Execute();            
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
            if (e.Button == MouseButton.Button2) LightningManager.spotlights[1].PointLight.BaseLight.Intensity = 2;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (isMouseDown && e.Button == MouseButton.Button1) isMouseDown = false;
            if (e.Button == MouseButton.Button2) LightningManager.spotlights[1].PointLight.BaseLight.Intensity = 0;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            WindowsWidth = e.Width;
            WindowsHeight = e.Height;
            if (isLoaded) mPersProj.ChangeWindowSize(WindowsWidth, WindowsHeight);
        }

        protected override void OnMove(WindowPositionEventArgs e)
        {
            base.OnMove(e);
            
        }

        protected override void OnFocusedChanged(FocusedChangedEventArgs e)
        {
            base.OnFocusedChanged(e);

            if (!e.IsFocused)
            {
                isFocused = false;
            }
            else
            {
                isFocused = true;
            }
        }

        protected override void OnClosed()
        {
            models.Clear();
            skybox.OnDelete();
            aim.OnDelete();
            //info.OnClear();
            

            CentralizedShaders.Dispose();

            base.OnClosed();
        }
    }
}