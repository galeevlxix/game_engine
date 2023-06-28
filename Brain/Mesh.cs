using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.Text.RegularExpressions;
using System.Globalization;

namespace game_2.Brain
{
    public class Mesh : IDisposable
    {
        private int VBO { get; set; }
        private int VAO { get; set; }
        private int IBO { get; set; }

        private float[] Vertices { get; set; }
        private int[] Indices { get; set; }

        Regex regOBJ = new Regex(@"\w*obj$");
        Regex regFBX = new Regex(@"\w*fbx$");
        Regex regPLY = new Regex(@"\w*ply$");
        Regex regDAE = new Regex(@"\w*dae$");

        public Mesh()
        {
            Vertices = new Storage().cubeVertices;
            Indices = new Storage().cubeIndices;

            Load();
        }

        public Mesh(string file_name)
        {
            if (regOBJ.IsMatch(file_name))
            {
                LoadFromObj(new StreamReader(file_name));
                Console.WriteLine("+obj");
            } 
            else if(regFBX.IsMatch(file_name))
            {

                Console.WriteLine("+fbx");
            }
            else if (regDAE.IsMatch(file_name))
            {
                Console.WriteLine("+dae");
            }
            else if (regPLY.IsMatch(file_name))
            {

                Console.WriteLine("+ply");
            }
            else
            {
                Vertices = new Storage().cubeVertices;
                Indices = new Storage().cubeIndices;
                Console.WriteLine("Unknown file format");
            }
            Load();
        }

        private void Load()
        {
            VBO = GL.GenBuffer();
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);
            IBO = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0); // 3 * sizeof(float)

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);

            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.DepthTest);
        }

        private void Clear()
        {
            if(VAO != 0)
            {
            }
        }

        public void Draw()
        {
            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(VBO);
            GL.DeleteVertexArray(VAO);

        }

        private void LoadFromObj(TextReader tr)
        {
            List<float> vertices = new List<float>();
            List<int> fig = new List<int>();

            vertices.Add(0.0f);
            vertices.Add(0.0f);
            vertices.Add(0.0f);

            string line;

            while ((line = tr.ReadLine()) != null)
            {
                line = line.Replace("  ", " ");
                var parts = line.Split(' ');

                if (parts.Length == 0) continue;
                switch (parts[0])
                {
                    case "v":
                        vertices.Add(float.Parse(parts[1], CultureInfo.InvariantCulture));
                        vertices.Add(float.Parse(parts[2], CultureInfo.InvariantCulture));
                        vertices.Add(float.Parse(parts[3], CultureInfo.InvariantCulture));
                        break;
                    case "f":
                        if (parts.Length == 4) 
                        {
                            foreach (string v in parts)
                            {
                                if (v != "f")
                                {
                                    var w = v.Split('/');
                                    fig.Add(int.Parse(w[0]));
                                }
                            }
                        }
                        if (parts.Length == 5)
                        {
                            var temp = new List<int>();

                            foreach (string v in parts)
                            {
                                if (v != "f")
                                {
                                    var w = v.Split('/');
                                    temp.Add(int.Parse(w[0]));
                                }
                            }


                            fig.Add(temp[0]);
                            fig.Add(temp[2]);
                            fig.Add(temp[3]);

                            fig.Add(temp[0]);
                            fig.Add(temp[1]);
                            fig.Add(temp[2]);
                        }
                        break;
                }
            }
            Vertices = vertices.ToArray();
            Indices = fig.ToArray();
        }

        private void LoadFromFbx(TextReader tr)
        {
            List<float> vertices = new List<float>();
            List<int> fig = new List<int>();


        }
    }
}
