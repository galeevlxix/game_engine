using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Diagnostics.SymbolStore;
using System.IO;

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
                LoadFromFbx(new StreamReader(file_name));
                Console.WriteLine("+fbx");
            }
            else if (regDAE.IsMatch(file_name))
            {
                LoadFromDae(new StreamReader(file_name));
                Console.WriteLine("+dae");
            }
            else if (regPLY.IsMatch(file_name))
            {
                LoadFromPly(new StreamReader(file_name));
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
                                    if (w[0] != "")
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
            bool ver = false, frag = false;
            Regex r1 = new Regex(@"^Vertices:\w*");
            Regex r2 = new Regex(@"^a:\w*");
            Regex r3 = new Regex(@"^PolygonVertexIndex:\w*");

            string line;

            while ((line = tr.ReadLine()) != null)
            {
                line = line.Replace(" ", "");
                line = line.Replace("\t", "");

                if (r1.IsMatch(line))
                {
                    ver = true;
                }
                else if (r3.IsMatch(line))
                {
                    frag = true;
                }
                else if (ver)
                {
                    if(line == "}")
                    {
                        ver = false;
                        continue;
                    }
                    if (r2.IsMatch(line)) line = line.Replace("a:", "");
                    line = line.Trim(',');
                    var w = line.Split(',');
                    foreach (string s in w)
                        if (s != "" && s != null)
                            vertices.Add(float.Parse(s, CultureInfo.InvariantCulture));
                }
                else if (frag)
                {
                    if (line == "}")
                    {
                        frag = false;
                        continue;
                    }
                    if (r2.IsMatch(line)) line = line.Replace("a:", "");
                    line = line.Trim(',');
                    var w = line.Split(',');
                    var temp = new List<int>();
                    foreach (string s in w)
                    {
                        if (s != "" && s != null)
                            temp.Add(int.Parse(s));
                        if (temp.Count == 4)
                        {
                            fig.Add(temp[0]);
                            fig.Add(temp[2]);
                            fig.Add(temp[3]);

                            fig.Add(temp[0]);
                            fig.Add(temp[1]);
                            fig.Add(temp[2]);
                            temp.Clear();
                        }
                    }
                }
            }

            Vertices = vertices.ToArray();
            Indices = fig.ToArray();
        }

        private void LoadFromDae(TextReader tr)  //не работает
        {
            List<float> vertices = new List<float>();
            List<int> fig = new List<int>();
            bool ver = false, frag = false, msh = false;

            Regex r = new Regex(@"<\w*>");
            Regex r1 = new Regex(@"^<p>\w*");
            char[] fdd = new char[] { '>', '<' };

            string line;

            while((line = tr.ReadLine()) != null)
            {
                line = line.Trim(' ');
                line = line.Replace("\t", "");
                if (line == "</mesh>")
                {
                    return;
                }
                else if (line == "<source id=\"TopN-mesh-positions\">")
                {
                    ver = true;
                }
                else if (r1.IsMatch(line))
                {
                    var t = line.Split(fdd);
                    var w = t[2].Split(' ');
                    foreach (string v in w)
                    {
                        if (v != "" && v != null)
                            fig.Add(int.Parse(v));
                    }
                }
                else if (ver)
                {
                    ver = false;
                    var t = line.Split(fdd);
                    var w = t[2].Split(' ');
                    foreach(string v in w)
                    {
                        if (v != "" && v != null)
                            vertices.Add(float.Parse(v, CultureInfo.InvariantCulture));
                    }
                }
            }
            Vertices = vertices.ToArray();
            Indices = fig.ToArray();
        }

        private void LoadFromPly(TextReader tr)
        {
            List<float> vertices = new List<float>();
            List<int> fig = new List<int>();
            string line;
            bool a = false;

            while ((line = tr.ReadLine()) != null)
            {
                line = line.Trim(' ');

                if (line == "end_header")
                {
                    a = true;
                }
                else if (a)
                {
                    var w = line.Split(' ');

                    if (w.Length == 8)
                    {
                        vertices.Add(float.Parse(w[0], CultureInfo.InvariantCulture));
                        vertices.Add(float.Parse(w[1], CultureInfo.InvariantCulture));
                        vertices.Add(float.Parse(w[2], CultureInfo.InvariantCulture));
                    }
                    else if(w.Length == 4)
                    {
                        if (w[0] == "3")
                        {
                            fig.Add(int.Parse(w[1]));
                            fig.Add(int.Parse(w[2]));
                            fig.Add(int.Parse(w[3]));
                        }
                        if (w[0] == "4")
                        {
                            var temp = new List<int>();

                            temp.Add(int.Parse(w[1]));
                            temp.Add(int.Parse(w[2]));
                            temp.Add(int.Parse(w[3]));
                            temp.Add(int.Parse(w[4]));

                            fig.Add(temp[0]);
                            fig.Add(temp[2]);
                            fig.Add(temp[3]);

                            fig.Add(temp[0]);
                            fig.Add(temp[1]);
                            fig.Add(temp[2]);
                        }
                    }
                }

            }
            Vertices = vertices.ToArray();
            Indices = fig.ToArray();
        }
    }
}
