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
        private MonochromeObject redLamp;
        private MonochromeObject blueLamp;

        LightingTechnique lightConfig;

        BaseLight baseLight;
        DirectionalLight directionalLight;

        PointLight[] pointLights = new PointLight[2];

        Spotlight[] spotlights = new Spotlight[2];

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

            //Console.WriteLine("Загрузка шрифта...");
            //info = new InfoPanel(InfoPanel.FontType.EnglishWithNumbers);

            Console.WriteLine("Загрузка света...");
            redLamp = new MonochromeObject(new vector3f(1, 0, 0), new vector3f(1, 1, 1));
            redLamp.pipeline.SetScale(0.5f);
            redLamp.pipeline.SetPosition(-5, 2, 0);

            blueLamp = new MonochromeObject(new vector3f(0, 1, 1), new vector3f(1, 1, 1));
            blueLamp.pipeline.SetScale(0.5f);
            blueLamp.pipeline.SetPosition(5, 2, 0);

            lightConfig = new LightingTechnique();

            //dirLight
            directionalLight.BaseLight.Color = new vector3f(1, 1, 1);
            directionalLight.BaseLight.DiffuseIntensity = 0.0f;
            directionalLight.BaseLight.AmbientIntensity = 0.1f;
            directionalLight.Direction = new vector3f(1, -0.2f, -1);

            //lightConfig.SetDirectionalLight(directionalLight);

            //specLight
            lightConfig.SetSpecular(Camera.Pos, 1, 16);

            //pointLights
            pointLights[0].Position = new vector3f(-5, 2, 0);
            pointLights[0].Attenuation.Exp = 0.0f;
            pointLights[0].Attenuation.Linear = 0.3f;
            pointLights[0].Attenuation.Constant = 1;
            pointLights[0].BaseLight.Color = new vector3f(1, 0, 0);
            pointLights[0].BaseLight.DiffuseIntensity = 1f;
            pointLights[0].BaseLight.AmbientIntensity = 0.0f;

            pointLights[1].Position = new vector3f(5, 2, 0);
            pointLights[1].Attenuation.Exp = 0.0f;
            pointLights[1].Attenuation.Linear = 0.3f;
            pointLights[1].Attenuation.Constant = 1;
            pointLights[1].BaseLight.Color = new vector3f(0, 1, 1);
            pointLights[1].BaseLight.DiffuseIntensity = 1f;
            pointLights[1].BaseLight.AmbientIntensity = 0.0f;

            //lightConfig.SetPointLights(pointLights);

            //spotlights
            spotlights[0].PointLight.Position = new vector3f(0, 1, 6);
            spotlights[0].PointLight.Attenuation.Exp = 0.0f;
            spotlights[0].PointLight.Attenuation.Linear = 0.1f;
            spotlights[0].PointLight.Attenuation.Constant = 1;
            spotlights[0].PointLight.BaseLight.Color = new vector3f(0, 1, 0);
            spotlights[0].PointLight.BaseLight.DiffuseIntensity = 0.7f;
            spotlights[0].Direction = new vector3f(1, -1, 0);
            spotlights[0].Cutoff = 0.1f;

            spotlights[1].PointLight.Position = new vector3f(0, 1, -6);
            spotlights[1].PointLight.Attenuation.Exp = 0.0f;
            spotlights[1].PointLight.Attenuation.Linear = 0.1f;
            spotlights[1].PointLight.Attenuation.Constant = 1;
            spotlights[1].PointLight.BaseLight.Color = new vector3f(1, 1, 0);
            spotlights[1].PointLight.BaseLight.DiffuseIntensity = 1f;
            spotlights[1].Direction = new vector3f(1, -1, 0);
            spotlights[1].Cutoff = 0.1f;

            lightConfig.SetSpotLights(spotlights);

            Console.WriteLine("Успешное завершение\n");
            loaded = true;
        }

        List<double> fps_out_list;
        double fps_out = 0;

        float counter = 0;

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
            //info.PutLineAndDraw( Math.Round(fps_out) + "fps" );

            CentralizedShaders.MonochromeShader.Use();


            float dt = (float)args.Time * 2;

            counter += dt;

            redLamp.pipeline.MoveX(math3d.sin(counter) * 5, dt);
            redLamp.pipeline.MoveY(math3d.sin(counter * 3) * 3, dt);
            redLamp.pipeline.MoveZ(-math3d.cos(counter) * 5, dt);
            redLamp.Draw();
            blueLamp.pipeline.MoveX(-math3d.sin(counter) * 5, dt);
            blueLamp.pipeline.MoveY(math3d.sin(counter * 3) * 3, dt);
            blueLamp.pipeline.MoveZ(math3d.cos(counter) * 5, dt);
            blueLamp.Draw();

            lightConfig.SetCameraPosition(Camera.Pos);

            pointLights[0].Position += new vector3f(math3d.sin(counter) * 5 * dt, math3d.sin(counter * 3) * 3 * dt, -math3d.cos(counter) * 5 * dt);
            pointLights[1].Position += new vector3f(-math3d.sin(counter) * 5 * dt, math3d.sin(counter * 3) * 3 * dt, math3d.cos(counter) * 5 * dt);

            //lightConfig.SetPointLights(pointLights);

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
           // info.OnClear();

            CentralizedShaders.Dispose();

            base.OnClosed();
        }
    }
}
