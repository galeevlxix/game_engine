using OpenTK.Graphics.OpenGL4;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;
using StbImageSharp;

namespace game_2.Brain
{
    public class Texture : IDisposable
    {
        public readonly int Handle;

        private TextureUnit textureUnit = TextureUnit.Texture0;

        private Texture(int glHandle, TextureUnit _unit)
        {
            Handle = glHandle;
            textureUnit = _unit;
        }

        public static Texture Load(string file_name, PixelInternalFormat pixel_format = PixelInternalFormat.Rgba, TextureUnit textureUnit = TextureUnit.Texture0, bool FlipVerticaly = true)
        {
            int handle = GL.GenTexture();   //Создаем

            GL.ActiveTexture(textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, handle);

            if (FlipVerticaly)
                StbImage.stbi_set_flip_vertically_on_load(1);

            using (Stream stream = File.OpenRead(file_name))
            {
                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                GL.TexImage2D(
                    TextureTarget.Texture2D,    //Тип создаваемой текстуры
                    0,                          //Уровень детализации
                    pixel_format,               //Формат для хранения пикселей на графическом процессоре
                    image.Width,                //Ширина изображения
                    image.Height,               //Высота изображения
                    0,                          //Граница изображения
                    PixelFormat.Rgba,           //Формат байтов   
                    PixelType.UnsignedByte,     //Тип пикселей
                    image.Data);                //Массив пикселей
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.Repeat);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return new Texture(handle, textureUnit);
        }

        public void Use()
        {
            GL.ActiveTexture(textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }

        public void Dispose() => GL.DeleteTexture(Handle);
    }
}
