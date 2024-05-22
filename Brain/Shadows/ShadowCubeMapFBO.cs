using game_2.MathFolder;
using OpenTK.Graphics.OpenGL;

namespace game_2.Brain.Shadows
{
    public class ShadowCubeMapFBO
    {
        private int m_shadowSize;

        private int m_fbo;
        private int m_shadowCubeMap; //файтический буфер глубены
        private int m_depthMap;

        public ShadowCubeMapFBO()
        {
            m_shadowSize = 512;

            m_fbo = 0; 
            m_shadowCubeMap = 0;
            m_depthMap = 0;

            Init();
        }

        private void Init()
        {
            // Создание FBO
            m_fbo = GL.GenFramebuffer();

            // Создание буфера глубины depth map
            m_depthMap = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, m_depthMap);

            GL.TexImage2D(
                TextureTarget.Texture2D, 
                0, 
                PixelInternalFormat.DepthComponent32,
                //or windowsize ??
                m_shadowSize,
                m_shadowSize, 
                0, 
                PixelFormat.DepthComponent, 
                PixelType.Float, 
                IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            GL.BindTexture(TextureTarget.Texture2D, 0);

            // создание cubemap
            m_shadowCubeMap = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap, m_shadowCubeMap);

            for (int i = 0; i < 6; i++)
            {
                GL.TexImage2D(
                    TextureTarget.TextureCubeMapPositiveX + i,
                    0,
                    PixelInternalFormat.R32f,
                    //or windowsize ??
                    m_shadowSize,
                    m_shadowSize,
                    0,
                    PixelFormat.Red,
                    PixelType.Float,
                    IntPtr.Zero);
            }

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, m_fbo);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, m_depthMap, 0);

            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);

            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
            {
                Console.WriteLine("ShadowMapFBO error: " + status.ToString());
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void BindForWriting(TextureTarget target)
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, m_fbo);
            GL.FramebufferTexture2D(FramebufferTarget.DrawFramebuffer, FramebufferAttachment.ColorAttachment0, target, m_shadowCubeMap, 0);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
        }

        public void BindForReading(TextureUnit unit) 
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.TextureCubeMap, m_shadowCubeMap);
        }

        public void Dispose()
        {
            GL.DeleteTexture(m_shadowCubeMap);
        }

        public int Size => m_shadowSize;
    }
}
