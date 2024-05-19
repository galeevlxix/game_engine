using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using game_2.Brain;
using OpenTK.Mathematics;
using game_2.Brain.SkyBoxFolder;
using game_2.Brain.AimFolder;
using game_2.Brain.Lights;
using game_2.Brain.Compiler;

namespace game_2
{
    public class GameEngine : GameWindow
    {
        private bool isMouseDown = false;
        private bool isHolding = false;

        private bool isLoaded = false;

        private int WindowWidth;
        private int WindowHeight;

        private Skybox skybox;
        private Aim aim;

        private readonly Color4 BackGroundColor;

        public GameEngine(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) 
        {
            WindowWidth = nativeWindowSettings.Size.X;
            WindowHeight = nativeWindowSettings.Size.Y;
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
            GL.Enable(EnableCap.CullFace);
            GL.FrontFace(FrontFaceDirection.Ccw);
            GL.CullFace(CullFaceMode.Back);

            CursorGrabbed = true;

            Camera.InitCamera();
            Camera.SetCameraPosition(8, 3, -10);

            CentralizedShaders.Load();
            
            aim = new Aim();

            ObjectArray.Init(WindowWidth, WindowHeight);

            skybox = new Skybox();

            LightningManager.Init();
            
            Console.WriteLine("Успешное завершение\n");
            isLoaded = true;

            await Task.Run(() => ConsoleCompiler.Run());
        }

        float deltaTime;
        // Рендер окна
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            deltaTime = (float)args.Time;

            FPSMeter.Update(args.Time);

            // управление камерой
            InputCallbacks(deltaTime);
            Camera.OnRender(deltaTime);

            // не нарушать последовательность !!!
            ObjectArray.OnRender(deltaTime);


            ObjectArray.DrawShadows();

            ObjectArray.SelectObjects();

            ObjectArray.DrawScene(isMouseDown);

            skybox.Draw();

            GL.CullFace(CullFaceMode.Front);

            aim.Draw();

            GL.CullFace(CullFaceMode.Back);

            LightningManager.Render(deltaTime);

            ConsoleCompiler.Execute();            
            SwapBuffers();
            GLFW.PollEvents();
        }

        private void InputCallbacks(float Time)
        {
            KeyboardState input = KeyboardState;
            if (input.IsKeyDown(Keys.Escape)) Close();

            //if (isMouseDown) mPersProj.ChangeFOV(25);
            //else mPersProj.ChangeFOV(50);       //ОПТИМИЗИРОВАТЬ

            Camera.OnMouse(-MouseState.Delta.X, -MouseState.Delta.Y);
            Camera.OnKeyboard(KeyboardState, Time);
        }

        // Callbacks
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Button1)
            {
                if (!isMouseDown && !isHolding)
                {
                    isMouseDown = true;
                    isHolding = true;
                }
                else if (isMouseDown && isHolding)
                {
                    isMouseDown = false;
                }
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButton.Button1)
            {
                if (isHolding || isMouseDown)
                {
                    isMouseDown = false;
                    isHolding = false;
                }
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            WindowWidth = e.Width;
            WindowHeight = e.Height;

            if (isLoaded) mPersProj.ChangeWindowSize(WindowWidth, WindowHeight);
            ObjectArray.WindowWidth = WindowWidth;
            ObjectArray.WindowHeight = WindowHeight;
            ObjectArray.Resize(WindowWidth, WindowHeight);
        }

        protected override void OnClosed()
        {
            ObjectArray.Clear();
            skybox.OnDelete();
            aim.OnDelete();

            CentralizedShaders.Dispose();

            base.OnClosed();
        }
    }
}