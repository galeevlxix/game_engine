using System.Globalization;
using System.Text.RegularExpressions;
using game_2.Storage;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Desktop;

namespace game_2.FileManagers
{
    public static class ModelLoader 
    {
        private static Regex regOBJ = new Regex(@"\w*obj$");
        private static Regex regFBX = new Regex(@"\w*fbx$");
        private static Regex regPLY = new Regex(@"\w*ply$");
        private static Regex regDAE = new Regex(@"\w*dae$");

        public static void LoadMesh(string file_name, out float[] Vertices, out int[] Indices)
        {
            Vertices = new float[0];
            Indices = new int[0];

            if (regOBJ.IsMatch(file_name))
            {
                LoadFromObj(new StreamReader(file_name), ref Vertices, ref Indices);
                Console.WriteLine("+obj");
                return;
            }
            else if (regFBX.IsMatch(file_name))
            {
                LoadFromFbx(new StreamReader(file_name), ref Vertices, ref Indices);
                Console.WriteLine("+fbx");
                return;
            }
            else if (regDAE.IsMatch(file_name))
            {
                LoadFromDae(new StreamReader(file_name), ref Vertices, ref Indices);
                Console.WriteLine("+dae");
                return;
            }
            else if (regPLY.IsMatch(file_name))
            {
                LoadFromPly(new StreamReader(file_name), ref Vertices, ref Indices);
                Console.WriteLine("+ply");
                return;
            }

            Vertices = Box.Vertices;
            Indices = Box.Indices;
            Console.WriteLine("Unknown file format");
        }

        private static void LoadFromObj(TextReader tr, ref float[] Vertices, ref int[] Indices)
        {
            List<float> _modelVerts = new List<float>();
            List<int> _modelInd = new List<int>();
            int _iter = 0;

            List<List<float>> vertCords = new List<List<float>>();

            List<List<float>> textCords = new List<List<float>>();

            List<List<float>> normCords = new List<List<float>>();

            int[] vertInd, texInd, normInd;

            string line;

            while ((line = tr.ReadLine()) != null)
            {
                line = line.Replace("  ", " ");
                var parts = line.Split(' ');

                if (parts.Length == 0) continue;
                switch (parts[0])
                {
                    case "v":
                        vertCords.Add(new List<float>
                        {
                            float.Parse(parts[1], CultureInfo.InvariantCulture),
                            float.Parse(parts[2], CultureInfo.InvariantCulture),
                            float.Parse(parts[3], CultureInfo.InvariantCulture)
                        });
                        break;
                    case "vt":
                        textCords.Add(new List<float>
                        {
                            float.Parse(parts[1], CultureInfo.InvariantCulture),
                            float.Parse(parts[2], CultureInfo.InvariantCulture)
                        });
                        break;
                    case "vn":
                        normCords.Add(new List<float>
                        {
                            float.Parse(parts[1], CultureInfo.InvariantCulture),
                            float.Parse(parts[2], CultureInfo.InvariantCulture),
                            float.Parse(parts[3], CultureInfo.InvariantCulture)
                        });
                        break;
                    case "f":
                        if (vertCords.Count == 0 || textCords.Count == 0 || normCords.Count == 0)
                        {
                            Console.WriteLine("Модель хуйня");
                            return;
                        }

                        List<string> fString = new List<string>();
                        for (int i = 0; i < parts.Length; i++)
                        {
                            if (parts[i] != "" && parts[i] != null && parts[i] != "f") fString.Add(parts[i]);
                        }

                        if (fString.Count == 3)
                        {
                            vertInd = new int[3];
                            texInd = new int[3];
                            normInd = new int[3];
                            int _i = 0;

                            foreach (string v in fString)
                            {
                                string[] w = v.Split('/');

                                vertInd[_i] = int.Parse(w[0]) - 1;
                                texInd[_i] = int.Parse(w[1]) - 1;
                                normInd[_i] = int.Parse(w[2]) - 1;
                                _i++;
                            }

                            for (int i = 0; i < 3; i++)
                            {
                                foreach (float v in vertCords[vertInd[i]])
                                {
                                    _modelVerts.Add(v);
                                }
                                foreach (float t in textCords[texInd[i]])
                                {
                                    _modelVerts.Add(t);
                                }
                                foreach (float n in normCords[normInd[i]])
                                {
                                    _modelVerts.Add(n);
                                }
                                _modelInd.Add(_iter++);
                            }                            
                        }
                        if (fString.Count == 4)
                        {
                            vertInd = new int[4];
                            texInd = new int[4];
                            normInd = new int[4];
                            int _i = 0;

                            var temp = new List<int>();

                            foreach (string v in parts)
                            {
                                string[] w = v.Split('/');

                                vertInd[_i] = int.Parse(w[0]) - 1;
                                texInd[_i] = int.Parse(w[1]) - 1;
                                normInd[_i] = int.Parse(w[2]) - 1;
                                _i++;
                            }

                            for (int i = 0; i < 4; i++)
                            {
                                foreach (float v in vertCords[vertInd[i]])
                                {
                                    _modelVerts.Add(v);
                                }
                                foreach (float t in textCords[texInd[i]])
                                {
                                    _modelVerts.Add(t);
                                }
                                foreach (float n in normCords[normInd[i]])
                                {
                                    _modelVerts.Add(n);
                                }
                            }

                            int i0 = _iter++;
                            int i1 = _iter++;
                            int i2 = _iter++;
                            int i3 = _iter++;

                            _modelInd.Add(i0);
                            _modelInd.Add(i2);
                            _modelInd.Add(i3);

                            _modelInd.Add(i0);
                            _modelInd.Add(i1);
                            _modelInd.Add(i2);
                        }
                        break;
                }
            }

            Vertices = _modelVerts.ToArray();
            Indices = _modelInd.ToArray();
        }

        private static void LoadFromFbx(TextReader tr, ref float[] Vertices, ref int[] Indices)
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
                    if (line == "}")
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

        private static void LoadFromDae(TextReader tr, ref float[] Vertices, ref int[] Indices)  //не работает
        {
            List<float> vertices = new List<float>();
            List<int> fig = new List<int>();
            bool ver = false, frag = false, msh = false;

            Regex r = new Regex(@"<\w*>");
            Regex r1 = new Regex(@"^<p>\w*");
            char[] fdd = new char[] { '>', '<' };

            string line;

            while ((line = tr.ReadLine()) != null)
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
                    foreach (string v in w)
                    {
                        if (v != "" && v != null)
                            vertices.Add(float.Parse(v, CultureInfo.InvariantCulture));
                    }
                }
            }
            Vertices = vertices.ToArray();
            Indices = fig.ToArray();
        }

        private static void LoadFromPly(TextReader tr, ref float[] Vertices, ref int[] Indices)
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
                    else if (w.Length == 4)
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
