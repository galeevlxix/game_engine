using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace game_2.Brain.Selecting
{
    public class SelectingMapFBO
    {
        private int m_fbo;
        private int m_selectMap;
        private int m_depthMap;

        public SelectingMapFBO()
        {
            m_fbo = 0;
            m_selectMap = 0;
            m_depthMap = 0;
        }
        
        public void Init(int WindowWidth, int WindowHeight)
        {
            // Создание FBO
            m_fbo = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, m_fbo);

            // Создание объекта текстуры для буфера с информацией о примитиве
            m_selectMap = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, m_selectMap);

            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgb32ui,
                WindowWidth,
                WindowHeight,
                0,
                PixelFormat.RgbInteger,
                PixelType.UnsignedInt,
                IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);

            GL.FramebufferTexture2D(
                FramebufferTarget.DrawFramebuffer, 
                FramebufferAttachment.ColorAttachment0, 
                TextureTarget.Texture2D, 
                m_selectMap, 
                0);

            // Создание объекта текстуры для буфера глубины
            m_depthMap = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, m_depthMap);

            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.DepthComponent,
                WindowWidth,
                WindowHeight,
                0,
                PixelFormat.DepthComponent,
                PixelType.Float,
                IntPtr.Zero);

            GL.FramebufferTexture2D(
                FramebufferTarget.DrawFramebuffer,
                FramebufferAttachment.DepthAttachment,
                TextureTarget.Texture2D,
                m_depthMap,
                0);

            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
            {
                Console.WriteLine("ShadowMapFBO error: " + status.ToString());
            }

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Enable()
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, m_fbo);
        }

        public void Disable()
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
        }

        // сюда точку в центре экрана
        public unsafe PixelInfo ReadPixel(int x, int y)
        {
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, m_fbo);
            GL.ReadBuffer(ReadBufferMode.ColorAttachment0);

            PixelInfo[] pixels = new PixelInfo[1];

            pixels[0] = new PixelInfo();
            
            GL.ReadPixels(x, y, 1, 1, PixelFormat.RgbInteger, PixelType.UnsignedInt, pixels);

            GL.ReadBuffer(ReadBufferMode.None);
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);

            return pixels[0];
        }

        public struct PixelInfo
        {
            public int ObjectID;
            public int DrawID;
            public int PrimID;

            public PixelInfo()
            {
                ObjectID = 0;
                DrawID = 0;
                PrimID = 0;
            }
        }
    }
}
