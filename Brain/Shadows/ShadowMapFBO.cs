using OpenTK.Graphics.OpenGL;

namespace game_2.Brain.Shadows
{
    public class ShadowMapFBO
    {
        private int m_shadowSize;

        private int m_fbo;
        private int m_shadowMap; //файтический буфер глубены

        public ShadowMapFBO()
        {
            m_shadowSize = 1024;

            m_fbo = 0; 
            m_shadowMap = 0;

            Init();
        }

        private void Init()
        {
            // Создание FBO
            m_fbo = GL.GenFramebuffer();

            // Создание буфера глубины
            m_shadowMap = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, m_shadowMap);
            GL.TexImage2D(
                TextureTarget.Texture2D, 
                0, 
                PixelInternalFormat.DepthComponent,
                m_shadowSize,
                m_shadowSize, 
                0, 
                PixelFormat.DepthComponent, 
                PixelType.Float, 
                IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode, (int)TextureCompareMode.CompareRefToTexture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareFunc, (int)DepthFunction.Lequal);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

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

        public int Size => m_shadowSize;
    }
}
