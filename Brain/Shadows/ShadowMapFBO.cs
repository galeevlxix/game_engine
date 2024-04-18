using game_2.MathFolder;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_2.Brain.Shadows
{
    public class ShadowMapFBO
    {
        private int m_shadowWidth;
        private int m_shadowHeight;

        private int m_fbo;
        private int m_shadowMap; //файтический буфер глубены

        public ShadowMapFBO(int width = 0, int height = 0)
        {
            m_shadowWidth = width;
            m_shadowHeight = height;

            m_fbo = 0; 
            m_shadowMap = 0;

            Init();
        }

        private void Init()
        {
            // Создание FBO
            m_fbo = GL.GenFramebuffer();

            // Создание буфера глубены
            m_shadowMap = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, m_shadowMap);
            GL.TexImage2D(
                TextureTarget.Texture2D, 
                0, 
                PixelInternalFormat.DepthComponent32,
                m_shadowWidth,
                m_shadowHeight, 
                0, 
                PixelFormat.DepthComponent, 
                PixelType.Float, 
                IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToBorder);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, m_fbo);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, m_shadowMap, 0);

            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);

            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
            {
                Console.WriteLine("ShadowMapFBO error: " + status.ToString());
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void BindForWriting()
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, m_fbo); // or draw framebuffer
        }

        public void BindForReading(TextureUnit unit) 
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, m_shadowMap);
        }

        public void Dispose()
        {
            GL.DeleteTexture(m_shadowMap);
        }
    }
}
