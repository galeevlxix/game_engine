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

        private ShadowMapFBO shadowMap;

        private readonly Color4 BackGroundColor;

        int shadow_size = 1024;

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
            //ObjectArray.Init();
            CentralizedShaders.SetValue(ShaderName.ShadowShader, "gShadowMap", 0);
            CentralizedShaders.SetValue(ShaderName.AssimpShader, "gMaterial.DiffuseMap", 0);
            CentralizedShaders.SetValue(ShaderName.AssimpShader, "gMaterial.NormalMap", 1);
            CentralizedShaders.SetValue(ShaderName.AssimpShader, "gMaterial.SpecularMap", 2);
            CentralizedShaders.SetValue(ShaderName.AssimpShader, "gShadowMap", 3);
            back = new AObject(ModelFolderPath + "obj_files\\background\\cube.obj");
            ball = new AObject(ModelFolderPath + "obj_files\\Ball\\ball1.obj");

            back.SetAngle(0, -90, 0);
            back.SetPosition(12, 5, 0);
            back.SetScale(5);

            ball.SetPosition(13, 1, 0);
            ball.SetAngle(0, 90, 0);
            ball.SetScale(5);

            shadowMap = new ShadowMapFBO(shadow_size, shadow_size);

            Console.WriteLine("Загрузка скайбокса...");
            skybox = new Skybox();

            Console.WriteLine("Загрузка света...");
            LightningManager.Init();

            Console.WriteLine("Успешное завершение\n");
            isLoaded = true;

            await Task.Run(() => ConsoleCompiler.Run());

        }
        private static string ModelFolderPath = "..\\..\\..\\Files\\Models\\";
        private AObject back;
        private AObject ball;

        // Рендер окна
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            FPSMeter.Update(args.Time);

            // управление камерой
            InputCallbacks(args.Time);
            Camera.OnRender((float)args.Time);

            ////// RENDER SHADOW //////////////////
            
            //GL.CullFace(CullFaceMode.Front);    
            GL.ClearColor(BackGroundColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            matrix4f viewMat = new matrix4f(), posMat = new matrix4f(), persMat = new matrix4f();
            viewMat.InitCameraTransform(-LightningManager.spotlights[0].Direction, vector3f.Up);
            vector3f pos = LightningManager.spotlights[0].PointLight.Position;
            posMat.InitTranslationTransform(-pos.x, -pos.y, -pos.z);
            persMat.InitPersProjTransform(45, shadow_size, shadow_size, 0.01f, 100);
            Matrix4 LightSpaceMatrix = (posMat * viewMat * persMat).ToOpenTK();

            CentralizedShaders.UseShader(ShaderName.ShadowShader);

            GL.Viewport(0, 0, shadow_size, shadow_size);
            shadowMap.BindForWriting();
            GL.Clear(ClearBufferMask.DepthBufferBit);

            //BALL
            Matrix4 world_ball = ball._pipeline.getWorld();
            Matrix4 ball_light_wvp = world_ball * LightSpaceMatrix;
            ball.Draw(ShaderName.ShadowShader, ball_light_wvp);

            //BACK
            Matrix4 world_back = back._pipeline.getWorld();
            Matrix4 back_light_wvp = world_back * LightSpaceMatrix;
            back.Draw(ShaderName.ShadowShader, back_light_wvp);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            ////// RENDER SCENE //////////////////////////

            //GL.CullFace(CullFaceMode.Back);
            // reset viewport
            GL.Viewport(0, 0, WindowWidth, WindowHeight);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            CentralizedShaders.UseShader(ShaderName.AssimpShader);

            persMat = mPersProj.PersProjMatrix;
            viewMat.InitCameraTransform(Camera.Target, Camera.Up);
            posMat.InitTranslationTransform(-Camera.Pos);
            Matrix4 SpaceMatrix = (posMat * viewMat * persMat).ToOpenTK();
            shadowMap.BindForReading(TextureUnit.Texture3);

            //BALL
            Matrix4 wvp = world_ball * SpaceMatrix;
            ball.Draw(ShaderName.AssimpShader, wvp, ball_light_wvp, world_ball);

            //BACK
            wvp = world_back * SpaceMatrix;
            back.Draw(ShaderName.AssimpShader, wvp, back_light_wvp, world_back);

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
            WindowWidth = e.Width;
            WindowHeight = e.Height;
            if (isLoaded) mPersProj.ChangeWindowSize(WindowWidth, WindowHeight);
        }

        protected override void OnClosed()
        {
           // ObjectArray.Clear();
            skybox.OnDelete();
            aim.OnDelete();
            //info.OnClear();

            CentralizedShaders.Dispose();

            base.OnClosed();
        }
    }
}