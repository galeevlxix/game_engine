using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using game_2.Brain;
using OpenTK.Mathematics;
using game_2.MathFolder;
using System.Diagnostics;
using OpenTK.Windowing.Common.Input;

namespace game_2
{
    public class GameEngine : GameWindow
    {
        bool isMouseDown;

        GameObj gameObj1;
        GameObj gameObj2;

        string ModelPath1;
        string ModelPath2;

        ObjectArray Models;

        int WindowsWidth;
        int WindowsHeight;

        Color4 BackGroundColor;

        Camera cam;

        public GameEngine(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) 
        {
            BackGroundColor = new Color4(0.102f, 0.102f, 0.153f, 1);
            ModelPath1 = "..\\..\\..\\Models\\obj_files\\WithPika.obj";
            ModelPath2 = "..\\..\\..\\Models\\SM_HandAxe.ply";
        }

        public void Init()
        {
            GLFWBindingsContext binding = new GLFWBindingsContext();
            GL.LoadBindings(binding);
            if (GLFW.Init())
            {
                Console.WriteLine("Успешная инициализация!");
            }
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(BackGroundColor);
            base.CursorGrabbed = true;
            ///////////////параметры игры

            Models = new ObjectArray();
            gameObj1 = new GameObj(ModelPath1);
            gameObj2 = new GameObj(ModelPath2);
            cam = new Camera();

            //Models.Add("axe", gameObj2);
            //Models.Add("girl", gameObj1);

            Models.Add("cube", new GameObj());

            GameTime.Start();
        }
        
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);            

            KeyboardState input = KeyboardState;
            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            if (input.IsKeyDown(Keys.P))
            {
                GameTime.PlayOrPause();
            }

            if (isMouseDown)
            {
                Models.ChangeFov(10);
            }
            else
            {
                Models.ChangeFov(50);
            }

            if (KeyboardState.IsKeyDown(Keys.LeftAlt))
            {
                GameTime.NextFaster();
            }
            else
            {
                GameTime.Next();
            }

            cam.OnMouse(-MouseState.Delta.X, -MouseState.Delta.Y);
            cam.OnKeyboard(KeyboardState);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            cam.OnRender();

            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            /*            Models["girl"].pipeline.Scale(0.1f);
                        Models["girl"].pipeline.Position(-2, 0, math3d.sin((GameTime.Time / 500) % (2 * math3d.PI)) * 5 - 10);
                        Models["girl"].pipeline.Rotate(-GameTime.Time / 3, 180, 0);

                        Models["axe"].pipeline.Scale(0.3f);
                        Models["axe"].pipeline.Position(2, 0f, -7);
                        Models["axe"].pipeline.Rotate(0, GameTime.Time / 50 + 180, 0);*/

            Models["cube"].pipeline.Scale(1f);
            Models["cube"].pipeline.Position(-2, 0, math3d.sin((GameTime.Time / 500) % (2 * math3d.PI)) * 5 - 10);
            Models["cube"].pipeline.Rotate(-GameTime.Time / 3, 180, 0);

            Models.SetCamera(cam);

            Models.Draw();
            SwapBuffers();
        }

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
            if (Models != null && Models.Count > 0)
            {
                Models.ChangeWindowSize(WindowsWidth, WindowsHeight);
            }
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            GameTime.End();
        }
    }
}
