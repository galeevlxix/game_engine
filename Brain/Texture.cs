using OpenTK.Graphics.OpenGL;
using System.Drawing;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using System.IO;
using StbImageSharp;

namespace game_2.Brain
{
    public class Texture
    {
        public readonly int Handle;

        public Texture(int glHandle)
        {
            Handle = glHandle;
        }

        public static Texture Load(string file_name)
        {
            int handle = GL.GenTexture();   //Создаем

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, handle);

            StbImage.stbi_set_flip_vertically_on_load(1);

            using (Stream stream = File.OpenRead(file_name))
            {
                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                GL.TexImage2D(TextureTarget.Texture2D, 
                    0, 
                    PixelInternalFormat.Rgba, 
                    image.Width, image.Height, 
                    0, 
                    PixelFormat.Rgba, 
                    PixelType.UnsignedByte, 
                    image.Data);
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return new Texture(handle);
        }

        public void Use(TextureUnit TextureUnit)
        {
            GL.ActiveTexture(TextureUnit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }
    }
}
