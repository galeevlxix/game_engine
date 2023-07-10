using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using game_2.Brain;
using OpenTK.Mathematics;
using game_2.MathFolder;

namespace game_2
{
    public class GameEngine : GameWindow
    {
        float speedX;
        float speedY;
        float speedZ;

        float angularX;
        float angularY;

        float velocity;
        float braking;
        float sensitivity;

        GameObj gameObj1;
        GameObj gameObj2;
        GameObj gameObj3;

        string ModelPath1;
        string ModelPath2;
        string ModelPath3;

        ObjectArray Models;

        int WindowsWidth;
        int WindowsHeight;

        Color4 BackGroundColor;

        public GameEngine(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) 
        {
            BackGroundColor = new Color4(0.102f, 0.102f, 0.153f, 1);
            ModelPath1 = "..\\..\\..\\Models\\obj_files\\WithPika.obj";
            ModelPath2 = "..\\..\\..\\Models\\SM_HandAxe.ply";
            ModelPath3 = "..\\..\\..\\Models\\fbx_files\\SM_FlatheadScrewdriver.fbx";
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
            base.CursorGrabbed = false;
            ///////////////параметры игры

            Models = new ObjectArray();

            gameObj1 = new GameObj(ModelPath1);
            gameObj2 = new GameObj(ModelPath2);
            gameObj3 = new GameObj(ModelPath3);

            Models.Add("girl", gameObj1);
/*            Models.Add("axe", gameObj2);
            Models.Add("screw", gameObj3);*/

            GameTime.Start();
            speedX = 0.00f;
            speedY = 0.00f;
            speedZ = 0.00f;
            angularX = 0;
            angularY = 0;
            velocity = 0.0001f;
            braking = 0.005f;
            sensitivity = 0.1f;
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);            

            KeyboardState input = KeyboardState;
            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            MoveCamera();
            RotateCamera();

            speedX *= 1 - braking;
            speedY *= 1 - braking;
            speedZ *= 1 - braking;

            angularX *= 1 - braking;
            angularY *= 1 - braking;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);
            GameTime.Next();

            Models["girl"].pipeline.Scale(1f, 1f, 1f);                
            Models["girl"].pipeline.Position(0, 0, -10);
            Models["girl"].pipeline.Rotate(0, GameTime.Time / 50 + 180, 0);
/*
            Models["axe"].pipeline.Scale(0.1f, 0.1f, 0.1f);               
            Models["axe"].pipeline.Position(2, -2f, -7);
            Models["axe"].pipeline.Rotate(0, GameTime.Time / 50, 0);

            Models["screw"].pipeline.Scale(0.1f, 0.1f, 0.1f);               
            Models["screw"].pipeline.Position(math3d.sin(GameTime.Time / 500 + 100) * 4, 0, -10);
            Models["screw"].pipeline.Rotate(0, -180, 0);*/


            Models.Draw();
            SwapBuffers();
        }

        public void MoveCamera()
        {
            if (KeyboardState.IsKeyDown(Keys.W))
            {
                speedZ += velocity;
            } 
            else if (KeyboardState.IsKeyDown(Keys.S))
            {
                speedZ -= velocity;
            }
            else if (KeyboardState.IsKeyDown(Keys.A))
            {
                speedX += velocity;
            }
            else if (KeyboardState.IsKeyDown(Keys.D))
            {
                speedX -= velocity;
            }
            else if (KeyboardState.IsKeyDown(Keys.Space))
            {
                speedY -= velocity;
            }
            else if (KeyboardState.IsKeyDown(Keys.LeftShift))
            {
                speedY += velocity;
            }
        }

        public void RotateCamera()
        {
            if (KeyboardState.IsKeyDown(Keys.Up))
            {
                angularX += velocity;
            }
            else if (KeyboardState.IsKeyDown(Keys.Down))
            {
                angularX -= velocity;
            }
            else if(KeyboardState.IsKeyDown(Keys.Up))
            {
                angularY += velocity;
            }
            else if(KeyboardState.IsKeyDown(Keys.Up))
            {
                angularY -= velocity;
            }
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            RotateCamera();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            WindowsWidth = e.Width;
            WindowsHeight = e.Height;
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            GameTime.End();
        }
    }
}
