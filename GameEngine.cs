using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using game_2.Brain;
using OpenTK.Mathematics;
using game_2.MathFolder;
using System.Security.Cryptography;

namespace game_2
{
    public class GameEngine : GameWindow
    {
        bool isMouseDown;

        ObjectArray Models;

        int WindowsWidth;
        int WindowsHeight;

        Color4 BackGroundColor;

        GameObj obj1;
        GameObj obj2;
        GameObj obj3;
        GameObj obj4;
        GameObj obj5;
        GameObj obj6;
        GameObj obj7;
        GameObj obj8;
        GameObj obj9;
        GameObj obj10;
        GameObj obj11;

        Camera cam;

        int BoxModel = 1;
        int FloorModel = 2;

        public GameEngine(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) 
        {
            BackGroundColor = new Color4(0.102f, 0.102f, 0.153f, 1);
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
            cam = new Camera();
            cam.Pos = new vector3f(0, 3, 4);

            obj1 = new GameObj(BoxModel);
            obj2 = new GameObj(FloorModel);
            obj3 = new GameObj(FloorModel);
            obj4 = new GameObj(FloorModel);
            obj5 = new GameObj(FloorModel);
            obj6 = new GameObj(FloorModel);
            obj7 = new GameObj(FloorModel);
            obj8 = new GameObj(FloorModel);
            obj9 = new GameObj(FloorModel);
            obj10 = new GameObj(FloorModel);
            obj11 = new GameObj(BoxModel);

            Models.Add(obj1); 
            Models.Add(obj2);
            Models.Add(obj3);
            Models.Add(obj4);
            Models.Add(obj5);
            Models.Add(obj6);
            Models.Add(obj7);
            Models.Add(obj8);
            Models.Add(obj9);
            Models.Add(obj10);
            Models.Add(obj11);
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

        float posx = -6, posz = -6;

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            cam.OnRender();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Models[0].pipeline.Scale(1f);
            Models[0].pipeline.Position(0, math3d.abs(math3d.sin(GameTime.Time/500) * 1.5f) + 0.97f, 0);
            Models[0].pipeline.Rotate(0, 0, 0);

            Models[1].pipeline.Scale(1f);
            Models[1].pipeline.Position(-6, 0, -6);
            Models[1].pipeline.Rotate(0, 0, 0);

            Models[2].pipeline.Scale(1f);
            Models[2].pipeline.Position(-6, 0, 0);
            Models[2].pipeline.Rotate(0, 0, 0);

            Models[3].pipeline.Scale(1f);
            Models[3].pipeline.Position(-6, 0, 6);
            Models[3].pipeline.Rotate(0, 0, 0);

            Models[4].pipeline.Scale(1f);
            Models[4].pipeline.Position(0, 0, -6);
            Models[4].pipeline.Rotate(0, 0, 0);

            Models[5].pipeline.Scale(1f);
            Models[5].pipeline.Position(0, 0, 0);
            Models[5].pipeline.Rotate(0, 0, 0);

            Models[6].pipeline.Scale(1f);
            Models[6].pipeline.Position(0, 0, 6);
            Models[6].pipeline.Rotate(0, 0, 0);

            Models[7].pipeline.Scale(1f);
            Models[7].pipeline.Position(6, 0, -6);
            Models[7].pipeline.Rotate(0, 0, 0);

            Models[8].pipeline.Scale(1f);
            Models[8].pipeline.Position(6, 0, 0);
            Models[8].pipeline.Rotate(0, 0, 0);

            Models[9].pipeline.Scale(1f);
            Models[9].pipeline.Position(6, 0, 6);
            Models[9].pipeline.Rotate(0, 0, 0);

            if (posz == -6f && posx + 0.01f <= 6f)
            {
                posx += 0.01f;
                if (posx >= 5.9f && posx <= 6f) posx = 6f;
            } 
            else if (posx == 6f && posz + 0.01f <= 6f)
            {
                posz += 0.01f;
                if (posz >= 5.9f && posz <= 6f) posz = 6f;
            } 
            else if (posz == 6f && posx - 0.01f >= -6f)
            {
                posx += -0.01f;
                if (posx <= -5.9f && posx >= -6f) posx = -6f;
            }
            else if (posx == -6f && posz - 0.01f >= -6f)
            {
                posz += -0.01f;
                if (posz <= -5.9f && posz >= -6f) posz = -6f;
            }

            Models[10].pipeline.Scale(1f);
            Models[10].pipeline.Position(posx, 1, posz);
            Models[10].pipeline.Rotate(0, 0, 0);

            Models.SetCamera(cam);

            Models.Draw();

            SwapBuffers();
            GLFW.PollEvents();
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

        protected override void OnClosed()
        {
            base.OnClosed();
            Models.Clear();
        }
    }
}
