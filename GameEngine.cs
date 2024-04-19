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
using game_2.Brain.Shadows;
using game_2.Brain.NewAssimpFolder;
using game_2.MathFolder;

namespace game_2
{
    public class GameEngine : GameWindow
    {
        private bool isMouseDown;
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
            GL.CullFace(CullFaceMode.Back);
            GL.FrontFace(FrontFaceDirection.Ccw);

            base.CursorGrabbed = true;

            ///////////////параметры игры

            Console.WriteLine("Загрузка камеры...");
            Camera.InitCamera();
            Camera.SetCameraPosition(8, 3, -10);
            

            Console.WriteLine("Загрузка шейдеров...");
            CentralizedShaders.Load();

            Console.WriteLine("Загрузка прицела...");
            aim = new Aim();

            Console.WriteLine("Загрузка моделей (assimp)...");
            ObjectArray.Init();
            ObjectArray.window_size_x = WindowWidth;
            ObjectArray.shadow_size_y = WindowHeight;

            CentralizedShaders.SetValue(ShaderName.AssimpShader, "gMaterial.DiffuseMap", 0);
            CentralizedShaders.SetValue(ShaderName.AssimpShader, "gMaterial.NormalMap", 1);
            CentralizedShaders.SetValue(ShaderName.AssimpShader, "gMaterial.SpecularMap", 2);
            CentralizedShaders.SetValue(ShaderName.AssimpShader, "gShadowMap", 3);

            Console.WriteLine("Загрузка скайбокса...");
            skybox = new Skybox();

            Console.WriteLine("Загрузка света...");
            LightningManager.Init();

            Console.WriteLine("Успешное завершение\n");
            isLoaded = true;

            await Task.Run(() => ConsoleCompiler.Run());

        }

        // Рендер окна
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            FPSMeter.Update(args.Time);

            // управление камерой
            InputCallbacks(args.Time);
            Camera.OnRender((float)args.Time);

            GL.CullFace(CullFaceMode.Front);
            GL.Viewport(0, 0, ObjectArray.shadow_size_x, ObjectArray.shadow_size_y);
            GL.Clear(ClearBufferMask.DepthBufferBit);
            ObjectArray.DrawShadows(LightningManager.spotlights[0]);

            GL.CullFace(CullFaceMode.Back);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, WindowWidth, WindowHeight);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            ObjectArray.Draw();

            skybox.Draw();

            aim.Draw();

            float dt = (float)args.Time * 2;

            LightningManager.Render(dt);

            ConsoleCompiler.Execute();            
            SwapBuffers();
            GLFW.PollEvents();
        }

        private void InputCallbacks(double Time)
        {
            KeyboardState input = KeyboardState;
            if (input.IsKeyDown(Keys.Escape)) Close();

            if (isMouseDown) mPersProj.ChangeFOV(25);
            else mPersProj.ChangeFOV(50);

            Camera.OnMouse(-MouseState.Delta.X, -MouseState.Delta.Y);
            Camera.OnKeyboard(KeyboardState, (float)Time);
        }

        // Callbacks
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (!isMouseDown && e.Button == MouseButton.Button1) isMouseDown = true;
            if (e.Button == MouseButton.Button2) LightningManager.spotlights[0].PointLight.BaseLight.Intensity = 0;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (isMouseDown && e.Button == MouseButton.Button1) isMouseDown = false;
            if (e.Button == MouseButton.Button2) LightningManager.spotlights[0].PointLight.BaseLight.Intensity = 2;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            WindowWidth = e.Width;
            WindowHeight = e.Height;
            if (isLoaded) mPersProj.ChangeWindowSize(WindowWidth, WindowHeight);
        }

        protected override void OnClosed()
        {
            ObjectArray.Clear();
            skybox.OnDelete();
            aim.OnDelete();
            //info.OnClear();

            CentralizedShaders.Dispose();

            base.OnClosed();
        }
    }
}