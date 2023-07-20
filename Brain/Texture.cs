using OpenTK.Graphics.OpenGL;
using System.Drawing;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using System.IO;
using StbImageSharp;
using System.Reflection.Metadata;

namespace game_2.Brain
{
    public class Texture
    {
        private readonly int Handle;

        private Texture(int glHandle)
        {
            Handle = glHandle;
        }

        public static Texture Load(string file_name)
        {
            int handle = GL.GenTexture();
            
            GL.BindTexture(TextureTarget.Texture2D, handle);

            StbImage.stbi_set_flip_vertically_on_load(1);

            using (Stream stream = File.OpenRead(file_name))
            {
                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                GL.TexImage2D(
                    TextureTarget.Texture2D,    //Тип создаваемой текстуры
                    0,                          //Уровень детализации
                    PixelInternalFormat.Rgba,   //Формат для хранения пикселей на графическом процессоре
                    image.Width,                //Ширина изображения
                    image.Height,               //Высота изображения
                    0,                          //Граница изображения
                    PixelFormat.Rgba,           //Формат байтов   
                    PixelType.UnsignedByte,     //Тип пикселей
                    image.Data);                //Массив пикселей
            }

            //Фильтрация
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            //Перенос
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            //генерируем коллекцию mipmapped
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return new Texture(handle);
        }

        public void Use(TextureUnit TextureUnit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(TextureUnit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }
    }
}
