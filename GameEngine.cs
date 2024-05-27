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
        private bool isMouseDown2 = false;

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

            Camera.InitCamera();
            Camera.SetCameraPosition(21, -4, 0);
            CursorGrabbed = true;

            CentralizedShaders.Load();
            
            aim = new Aim();

            LightningManager.Init();

            CentralizedShaders.SetValue(ShaderName.AssimpShader, "TurnOnShadows", 1);
            ObjectArray.Init(WindowWidth, WindowHeight);

            skybox = new Skybox();
            
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
            InputCallbacks(deltaTime);

            // управление камерой

            InputCallbacksCamera(deltaTime);

            // не нарушать последовательность !!!
            ObjectArray.OnRender(deltaTime);

            ObjectArray.DrawShadows();

            ObjectArray.GetObservedObject();

            ObjectArray.GetPickedObject(mouse_shooter);

            ObjectArray.DrawScene();

            skybox.Draw();

            GL.CullFace(CullFaceMode.Front);

            if (!ObjectArray.pick_mode && !isMouseDown2) aim.Draw();

            GL.CullFace(CullFaceMode.Back);

            LightningManager.Render(deltaTime);

            ConsoleCompiler.Execute();
            SwapBuffers();
            GLFW.PollEvents();
        }

        short mouse_shooter;

        private void InputCallbacks(float Time)
        {
            if (KeyboardState.IsKeyDown(Keys.Escape)) Close();
            if (isMouseDown && mouse_shooter <= 1) mouse_shooter++;

            if (ObjectArray.pick_mode)
            {
                ObjectArray.RotatePickedObject(MouseState.Delta.X / 16);
            }
        }

        private void InputCallbacksCamera(float Time)
        {
            if (!ObjectArray.pick_mode)
            {
                Camera.OnMouse(-MouseState.Delta.X, -MouseState.Delta.Y);
                Camera.OnKeyboard(KeyboardState, Time);

                Camera.onPositionRender(deltaTime);
                Camera.onAngleRender(deltaTime);
            }
            else
            {
                Camera.OnMouse(0, -MouseState.Delta.Y / 4);
                Camera.onAngleRender(deltaTime);
            }
        }

        // Callbacks
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButton.Button1)
            {
                if (!isMouseDown)
                {
                    mouse_shooter = 0;
                    isMouseDown = true;
                }
            }
            if (e.Button == MouseButton.Button2)
            {
                if (!isMouseDown2)
                {
                    isMouseDown2 = true;
                    mPersProj.ChangeFOV(30);
                }
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButton.Button1)
            {
                if (isMouseDown)
                {
                    isMouseDown = false;
                }
            }
            if (e.Button == MouseButton.Button2)
            {
                if (isMouseDown2)
                {
                    isMouseDown2 = false;
                    mPersProj.ChangeFOV(70);
                }
            }
        }

        float prevWheelPos = 0;
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            if (ObjectArray.pick_mode)
            {
                if (e.OffsetY - prevWheelPos > 0)
                {
                    ObjectArray.ScaleOfPickedObject += 0.001f;
                }
                else
                {
                    if (ObjectArray.ScaleOfPickedObject - 0.001f >= 0)
                        ObjectArray.ScaleOfPickedObject -= 0.001f;
                }
            }

            prevWheelPos = e.OffsetY;
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