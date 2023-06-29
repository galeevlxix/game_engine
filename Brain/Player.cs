using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_2.Brain
{
    public class Player : PhysObj
    {
        private float CameraRotX { get; set; }
        private float CameraRotY { get; set; }

        public Player() { }

        public override void Reset()
        {
            base.Reset();
            CameraRotX = 0;
            CameraRotY = 0;
        }

        public override void Update(MouseState mouse, KeyboardState keyboard)
        {
            base.Update(mouse, keyboard);
            Look(mouse.Delta.X, mouse.Delta.Y);
            float moveF = 0.0f, moveL = 0.0f;

            if (keyboard.IsKeyDown(Keys.W))
                moveF += 1.0f;

            if (keyboard.IsKeyDown(Keys.S))
                moveF -= 1.0f;

            if (keyboard.IsKeyDown(Keys.A))
                moveL += 1.0f;

            if (keyboard.IsKeyDown(Keys.D))
                moveL -= 1.0f;

            Move(moveF, moveL);
        }

        public void Move(float f, float l)
        {
            float speed = 2.0f;
            // in order to move forward in the direction of the camera, some matrix magic is needed
            // im not entirely sure how this works but it does lol
            Matrix4 camToWorld = LocalToWorld() * Pipeline.RotateY(CameraRotY);
            velocity += math3d.MultiplyDirection(camToWorld, new vector3(l, 0, f)) * (speed * GameTime.Delta);
        }

        public void Look(float x, float y)
        {
            float sensitivity = 0.001f;
            // the reason you take Y from X is due to the matrix stuff,
            // the world is a bit inverted basically, so Y affects X

            CameraRotX -= y * sensitivity;
            CameraRotY -= x * sensitivity;

            // sort of clamp the X and Y looking
            if (CameraRotX > (MathF.PI / 2))
            {
                CameraRotX = MathF.PI / 2;
            }
            else if (CameraRotX < ((-MathF.PI) / 2))
            {
                CameraRotX = ((-MathF.PI) / 2);
            }

            if (CameraRotY > MathF.PI)
            {
                CameraRotY -= MathF.PI * 2;
            }
            else if (CameraRotY < (-MathF.PI))
            {
                CameraRotY += MathF.PI * 2;
            }
        }

        public Matrix4 WorldToCamera()
        {
            return
                Pipeline.RotateX(-CameraRotX) *
                Pipeline.RotateY(-CameraRotY) *
                WorldToLocal();
        }
    }
}
