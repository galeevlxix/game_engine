using OpenTK.Graphics.OpenGL;

namespace game_2.Brain.Shadows
{
    public class ShadowMapFBO
    {
        private int shadowMapSize;

        private int FBO;
        private int depthMap;

        public ShadowMapFBO()
        {
            shadowMapSize = 1024;

            FBO = 0; 
            depthMap = 0;

            Init();
        }

        private void Init()
        {
            // Создание FBO
            FBO = GL.GenFramebuffer();

            // Создание объекта текстуры для буфера глубины
            depthMap = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, depthMap);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, shadowMapSize, shadowMapSize, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode, (int)TextureCompareMode.CompareRefToTexture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareFunc, (int)DepthFunction.Lequal);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, depthMap, 0);

            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);
            
            // Проверка успеха инициализации и развязка от текстуры и буфера глубины
            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
            {
                Console.WriteLine("Ошибка инициализации shadowMapFBO: " + status.ToString());
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void BindForWriting()
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, FBO);
        }

        public void BindForReading(TextureUnit unit) 
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, depthMap);
        }

        public void Dispose()
        {
            GL.DeleteTexture(depthMap);
        }

        public int Size => shadowMapSize;
    }
}
