using game_2.FileManagers;
using game_2.MathFolder;
using OpenTK.Windowing.Desktop;
using System.Globalization;

namespace game_2
{
    class Program
    {
        static void Main(string[] args)
        {
            GameWindowSettings settings = GameWindowSettings.Default;
            NativeWindowSettings windowSettings = NativeWindowSettings.Default;

            windowSettings.WindowState = OpenTK.Windowing.Common.WindowState.Maximized;
            Console.WriteLine(windowSettings.CurrentMonitor.Pointer.ToString());

            windowSettings.Title = "Game";

            GameEngine engine = new GameEngine(settings, windowSettings);
            engine.Init();
            engine.Run();
        }

        //функция для создания нового файла obj с нормалями из старого файла obj без нормалей. UPD: Больше не используется.
        private static void addNormalsToObjFile(string oldFilePath, string newFilePath)
        {
            TextReader reader = new StreamReader(oldFilePath);
            string? line;

            string faceSector = "";
            string normalSector = "";

            List<List<float>> vertCords = new List<List<float>>();
            int normalCounter = 0;

            File.Delete(newFilePath);
            using (StreamWriter sw = new StreamWriter(newFilePath))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(' ');

                    switch (parts[0])
                    {
                        case "g":       //сохранить в faceSector
                        case "usemtl":
                            faceSector += line + "\n";
                            break;
                        case "v":       //сохранить в массиве и положить в файл
                            vertCords.Add(new List<float> {
                                float.Parse(parts[1], CultureInfo.InvariantCulture),
                                float.Parse(parts[2], CultureInfo.InvariantCulture),
                                float.Parse(parts[3], CultureInfo.InvariantCulture) });
                            sw.WriteLine(line);
                            break;
                        case "f":       //parts разбита на вершины. разбить каждую на индексы. найти координаты вершин и по ним найти нормаль. 
                            normalCounter++;
                            List<int> v_inds = new List<int>();
                            for (int i = 1; i < parts.Length; i++)
                            {
                                v_inds.Add(int.Parse(parts[i].Split('/')[0]) - 1);
                            }

                            vector3f vNormal = new vector3f();

                            //если вектор нормали создать не удалось, крутим грань пока не получится. иначе сохраняем нормаль.
                            for (int i = 0; i < v_inds.Count; i++)
                            {
                                int i2 = (i + 1) % v_inds.Count;
                                int i3 = (i + 2) % v_inds.Count;
                                ModelLoader.CalcNormals(vertCords[v_inds[i]], vertCords[v_inds[i2]], vertCords[v_inds[i3]], out vector3f normal);
                                if (normal.x.ToString("0.000", CultureInfo.InvariantCulture) != "NaN" && 
                                    normal.y.ToString("0.000", CultureInfo.InvariantCulture) != "NaN" && 
                                    normal.z.ToString("0.000", CultureInfo.InvariantCulture) != "NaN")
                                {
                                    vNormal = normal;
                                    break;
                                }
                            }

                            normalSector += "vn " + vNormal.x.ToString("0.000", CultureInfo.InvariantCulture) + " " + vNormal.y.ToString("0.000", CultureInfo.InvariantCulture) + " " + vNormal.z.ToString("0.000", CultureInfo.InvariantCulture) + "\n";

                            string newFaceLine = "f ";
                            for (int i = 1; i < parts.Length; i++)
                            {
                                newFaceLine += parts[i] + "/" + normalCounter + " ";
                            }

                            faceSector += newFaceLine + "\n";

                            break;
                        default:        //vt, mtllib, # просто положить в файл
                            sw.WriteLine(line);
                            break;
                    }
                }

                sw.WriteLine(normalSector);
                sw.WriteLine(faceSector);
                sw.Close();
                reader.Close();
            }            
        }

        //функция для создания нового скомпрессированного файла obj. UPD: Больше не используется.
        private static void CompressObjFile(string oldFilePath, string newFilePath)
        {            
            string? line;

            string faceSector = "";
            string verticesSector = "";

            List<string> old_vertices = new List<string>();
            List<string> old_text_cords = new List<string>();
            List<string> old_normals = new List<string>();

            using (TextReader reader = new StreamReader(oldFilePath))
            {
                bool exit = false;
                while ((line = reader.ReadLine()) != null)
                {
                    if (exit) break;

                    line = line.Trim();
                    line = line.Replace("  ", " ");

                    string[] parts = line.Split(' ');
                    switch (parts[0])
                    {
                        case "v":
                            old_vertices.Add(line);
                            break;
                        case "vt":
                            old_text_cords.Add(line);
                            break;
                        case "vn":
                            old_normals.Add(line);
                            break;
                        case "f":
                            //exit = true;
                            break;
                    }
                }
            }

            Dictionary<string, int> vert = new Dictionary<string, int>();
            Dictionary<string, int> text = new Dictionary<string, int>();
            Dictionary<string, int> norm = new Dictionary<string, int>();

            int i = 1;
            foreach (string vert_line in old_vertices)
            {
                if (!vert.ContainsKey(vert_line))
                {
                    vert.Add(vert_line, i++);
                }
            }

            i = 1;
            foreach (string text_line in old_text_cords)
            {
                if (!text.ContainsKey(text_line))
                {
                    text.Add(text_line, i++);
                }
            }

            i = 1;
            foreach (string norm_line in old_normals)
            {
                if (!norm.ContainsKey(norm_line))
                {
                    norm.Add(norm_line, i++);
                }
            }

            using (TextReader reader = new StreamReader(oldFilePath))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    string[] parts = line.Split(' ');
                    switch (parts[0])
                    {
                        case "v":
                        case "vt":
                        case "vn":
                            break;
                        case "f":
                            line = line.Trim('f').Trim();
                            string[] f_parts = line.Split(' ');
                            string newline = "f";

                            foreach (string f_part in f_parts)
                            {
                                string[] _f = f_part.Split('/');

                                int v_ind = vert[old_vertices[int.Parse(_f[0]) - 1]];
                                int t_ind = text[old_text_cords[int.Parse(_f[1]) - 1]];
                                int n_ind = norm[old_normals[int.Parse(_f[2]) - 1]];

                                newline += " " + v_ind + "/" + t_ind + "/" + n_ind; 
                            }
                            faceSector += newline + "\n";
                            break;

                        case "g":       //сохранить в faceSector
                        case "s":
                        case "usemtl":
                            faceSector += line + "\n";
                            break;
                        default:
                            verticesSector += line + "\n";
                            break;
                    }
                }
            }

            File.Delete(newFilePath);

            using (StreamWriter sw = new StreamWriter(newFilePath))
            {
                sw.WriteLine(verticesSector);

                foreach (string v_line in vert.Keys)
                {
                    sw.WriteLine(v_line);
                }

                foreach (string t_line in text.Keys)
                {
                    sw.WriteLine(t_line);
                }

                foreach (string n_line in norm.Keys)
                {
                    sw.WriteLine(n_line);
                }

                sw.WriteLine(faceSector);

                sw.Close();
            }
        }
    }
}
