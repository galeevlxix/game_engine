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

        int shadow_size_x = 1024;
        int shadow_size_y = 1024;

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
            //ObjectArray.Init();
            CentralizedShaders.SetValue(ShaderName.AssimpShader, "gMaterial.DiffuseMap", 0);
            CentralizedShaders.SetValue(ShaderName.AssimpShader, "gMaterial.NormalMap", 1);
            CentralizedShaders.SetValue(ShaderName.AssimpShader, "gMaterial.SpecularMap", 2);
            CentralizedShaders.SetValue(ShaderName.AssimpShader, "gShadowMap", 3);
            back = new AObject(ModelFolderPath + "obj_files\\background\\cube.obj");
            ball = new AObject(ModelFolderPath + "obj_files\\Ball\\ball1.obj");

            back.SetAngle(0, -90, 0);
            back.SetPosition(12, 5, 0);
            back.SetScale(5);

            ball.SetPosition(15, 1, 0);
            ball.SetAngle(0, 90, 0);
            ball.SetScale(5);

            shadowMap = new ShadowMapFBO(shadow_size_x, shadow_size_y);

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



            ////// PARAMETERS //////////////////

            var p = new matrix4f();
            p.InitPersProjTransform(120, shadow_size_x, shadow_size_y, 0.1f, 100);
            Matrix4 projMatrixFromLight = p.ToOpenTK();
            //projMatrixFromLight.Transpose();
            Matrix4 projMatrix = mPersProj.PersProjMatrix.ToOpenTK();
            
            //projMatrix.Transpose();

            vector3f pos = LightningManager.spotlights[0].PointLight.Position;
            vector3f tar = LightningManager.spotlights[0].Direction;
            tar = new vector3f(tar.x, tar.y, tar.z);

            matrix4f LightSpacePos = new matrix4f();
            LightSpacePos.InitTranslationTransform(-pos);
            matrix4f LightSpaceTarget = new matrix4f();
            LightSpaceTarget.InitCameraTransform(-tar, vector3f.Up);

            Matrix4 viewMatrixFromLight = (LightSpacePos * LightSpaceTarget).ToOpenTK();
            //viewMatrixFromLight.Transpose();
            Matrix4 viewMatrix = (Camera.CameraTranslation * Camera.CameraRotation).ToOpenTK();

            Shader shadowShader = CentralizedShaders.GetShader(ShaderName.ShadowShader);
            Shader normalShader = CentralizedShaders.GetShader(ShaderName.AssimpShader);

            ////// RENDER SHADOW //////////////////

            //GL.CullFace(CullFaceMode.Front);
            shadowMap.BindForWriting();
            GL.Viewport(0, 0, shadow_size_x, shadow_size_y);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            shadowShader.Use();
            
            Draw(shadowShader, ball, viewMatrixFromLight, projMatrixFromLight);
            Draw(shadowShader, back, viewMatrixFromLight, projMatrixFromLight);

            Matrix4 mvpMatrixFromLight_ball = ball._pipeline.getWorld() * viewMatrixFromLight * projMatrixFromLight;
            Matrix4 mvpMatrixFromLight_back = back._pipeline.getWorld() * viewMatrixFromLight * projMatrixFromLight;

            ////// RENDER SCENE //////////////////////////
            //GL.CullFace(CullFaceMode.Back);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, WindowWidth, WindowHeight);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            normalShader.Use();
            shadowMap.BindForReading(TextureUnit.Texture3);

            normalShader.setValue("light_wvp", mvpMatrixFromLight_ball);
            //viewMatrix, projMatrix
            //viewMatrixFromLight, projMatrixFromLight
            Draw(normalShader, ball, viewMatrix, projMatrix);
            normalShader.setValue("light_wvp", mvpMatrixFromLight_back);
            Draw(normalShader, back, viewMatrix, projMatrix);
            skybox.Draw();

            aim.Draw();

            float dt = (float)args.Time * 2;

            LightningManager.Render(dt);

            ConsoleCompiler.Execute();            
            SwapBuffers();
            GLFW.PollEvents();
        }

        private void Draw(Shader program, AObject o, Matrix4 viewMatrix, Matrix4 projMatrix)
        {
            o.Draw(program, viewMatrix, projMatrix);
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
           // ObjectArray.Clear();
            skybox.OnDelete();
            aim.OnDelete();
            //info.OnClear();

            CentralizedShaders.Dispose();

            base.OnClosed();
        }
    }
}