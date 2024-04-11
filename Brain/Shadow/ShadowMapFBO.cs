using OpenTK.Graphics.OpenGL4;

namespace game_2.Brain.Shadow
{
    public class ShadowMapFBO
    {
        private int m_fbo;
        private int m_shadowMap;

        public ShadowMapFBO() 
        {
            m_fbo = 0;
            m_shadowMap = 0;
        }

        public void Init(int width, int height)
        {
            // Создаем FBO
            m_fbo = GL.GenFramebuffer();

            // Создаем буфер глубины
            m_shadowMap = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, m_shadowMap);

            GL.TexImage2D(
                    TextureTarget.Texture2D,            //Тип создаваемой текстуры
                    0,                                  //Уровень детализации
                    PixelInternalFormat.DepthComponent, //Формат для хранения пикселей на графическом процессоре
                    width,                              //Ширина изображения
                    height,                             //Высота изображения
                    0,                                  //Граница изображения
                    PixelFormat.DepthComponent,         //Формат байтов   
                    PixelType.Float,                    //Тип пикселей
                    IntPtr.Zero);                       //Массив пикселей

            GL.TexParameter(
                TextureTarget.Texture2D, 
                TextureParameterName.TextureMinFilter, 
                (int)TextureMinFilter.Nearest);

            GL.TexParameter(
                TextureTarget.Texture2D,
                TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Nearest);

            GL.TexParameter(
                TextureTarget.Texture2D,
                TextureParameterName.TextureWrapS,
                (int)TextureWrapMode.ClampToEdge);

            GL.TexParameter(
                TextureTarget.Texture2D,
                TextureParameterName.TextureWrapT,
                (int)TextureWrapMode.ClampToEdge);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, m_fbo);

            GL.FramebufferTexture2D(
                FramebufferTarget.Framebuffer, 
                FramebufferAttachment.DepthAttachment, 
                TextureTarget.Texture2D, 
                m_shadowMap, 
                0); 

            // Отключаем запись в буфер цвета
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);

            FramebufferErrorCode errorCode = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);

            if (errorCode != FramebufferErrorCode.FramebufferComplete)
            {
                Console.WriteLine("Shadow Buffer Error \n" + errorCode);
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void BindForWriting()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, m_fbo);
        }

        public void BindForReading(TextureUnit unit)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, m_shadowMap);
        }

        public void Dispose()
        {
            GL.DeleteFramebuffer(m_fbo);
            GL.DeleteFramebuffer(m_shadowMap);
        }
    }
}
