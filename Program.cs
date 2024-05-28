using game_2.FileManagers;
using game_2.MathFolder;
using OpenTK.Mathematics;
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

            string dir = "C:\\Users\\Lenovo\\source\\repos\\game_2\\Files\\Models\\Museums\\";

            //GoAllProcedures(dir + "st3\\goat");

            windowSettings.WindowState = OpenTK.Windowing.Common.WindowState.Maximized;
            windowSettings.Size = new Vector2i(1920, 1080);

            windowSettings.Title = "Game";

            GameEngine engine = new GameEngine(settings, windowSettings);
            engine.Init();
            engine.Run();
        }

        private static void GoAllProcedures(string filename)
        {
            /*
            addNormalsToObjFile(
                filename + ".obj",
                filename + "_norm.obj");

            Console.WriteLine("+Normals");
            */

            CompressObjFile(
                filename + ".obj",
                filename + "_comp.obj");

            Console.WriteLine("+Compressed");            

            NormalizeObjectSizeOnX(
                filename + "_comp.obj",
                filename + "_fin.obj");

            Console.WriteLine("+Normalized");

            RotateObject(
                filename + "_fin.obj",
                filename + "_rot.obj", 
                new vector3f(0, 45, 0));
        }

        private static void MakeMTLFile(string OldPath)
        {
            string dir = Path.GetDirectoryName(OldPath);
            string name = Path.GetFileNameWithoutExtension(OldPath);
            string format = Path.GetExtension(OldPath);

            string NewPath = dir + "\\" + name + "_D" + format;

            using (StreamReader reader = new StreamReader(OldPath))
            {
                using (StreamWriter writer = new StreamWriter(NewPath))
                {
                    string? line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.Trim();
                        line = line.Replace("  ", " ");
                        string[] parts = line.Split(' ');

                        switch (parts[0])
                        {
                            case "newmtl":
                                writer.WriteLine();
                                writer.WriteLine(line);
                                break;
                            case "Ns":
                            case "map_Kd":
                            case "map_Kn":
                            case "map_Ks":
                            case "#":
                                writer.WriteLine(line);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        //функция для создания нового файла obj с нормалями из старого файла obj без нормалей. UPD: Больше не используется.
        private static void addNormalsToObjFile(string oldFilePath, string newFilePath)
        {
            TextReader reader = new StreamReader(oldFilePath);
            string? line;

            string tempFilePath1 = Path.GetDirectoryName(oldFilePath) + "\\tempFile1.obj";
            string tempFilePath2 = Path.GetDirectoryName(oldFilePath) + "\\tempFile2.obj";

            StreamWriter faceSectorFile = new StreamWriter(tempFilePath1);
            StreamWriter normalSectorFile = new StreamWriter(tempFilePath2);

            string normalSector = "";

            List<List<float>> vertCords = new List<List<float>>();
            int normalCounter = 0;

            float lines_count = File.ReadAllLines(oldFilePath).Length;

            File.Delete(newFilePath);
            using (StreamWriter sw = new StreamWriter(newFilePath))
            {
                float current_line = 1;
                int persent = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    int current_persent = (int)(current_line / lines_count * 100);
                    if (current_persent > persent)
                    {
                        //Console.Clear();
                        persent = (int)(current_line / lines_count * 100);
                        if (persent % 10 == 0) Console.WriteLine("Чтение файла: " + persent + "%");
                    }
                    current_line++;

                    line = line.Trim();
                    line = line.Replace("  ", " ");
                    string[] parts = line.Split(' ');
                    
                    switch (parts[0])
                    {
                        case "g":       //сохранить в faceSector
                        case "usemtl":
                            faceSectorFile.WriteLine(line);
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

                            normalSector = "vn " + vNormal.x.ToString("0.000", CultureInfo.InvariantCulture) + " " + vNormal.y.ToString("0.000", CultureInfo.InvariantCulture) + " " + vNormal.z.ToString("0.000", CultureInfo.InvariantCulture);
                            normalSectorFile.WriteLine(normalSector);

                            string newFaceLine = "f ";
                            for (int i = 1; i < parts.Length; i++)
                            {
                                newFaceLine += parts[i] + "/" + normalCounter + " ";
                            }

                            faceSectorFile.WriteLine(newFaceLine);

                            break;
                        default:        //vt, mtllib, # просто положить в файл
                            sw.WriteLine(line);
                            break;
                    }
                }

                faceSectorFile.Close();
                normalSectorFile.Close();

                StreamReader normalSectorFileReader = new StreamReader(tempFilePath2);
                StreamReader faceSectorFileReader = new StreamReader(tempFilePath1);

                while ((line = normalSectorFileReader.ReadLine()) != null)
                {
                    sw.WriteLine(line);
                }
                normalSectorFileReader.Close();

                while ((line = faceSectorFileReader.ReadLine()) != null)
                {
                    sw.WriteLine(line);
                }
                faceSectorFileReader.Close();

                sw.Close();
                reader.Close();
            }            
        }

        //функция для создания нового скомпрессированного файла obj. UPD: Больше не используется.
        private static void CompressObjFile(string oldFilePath, string newFilePath)
        {            
            string? line;
            string tempFilePath = Path.GetDirectoryName(oldFilePath) + "\\tempFile.obj";
            StreamWriter faceSectorFile = new StreamWriter(tempFilePath);
            string verticesSector = "";


            List<string> old_vertices = new List<string>();
            List<string> old_text_cords = new List<string>();
            List<string> old_normals = new List<string>();

            float lines_count = File.ReadAllLines(oldFilePath).Length;

            using (StreamReader reader = new StreamReader(oldFilePath))
            {
                bool exit = false;
                float current_line = 1;
                int persent = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    if (exit) break;
                    
                    int current_persent = (int)(current_line / lines_count * 100);
                    if (current_persent > persent)
                    {
                        //Console.Clear();
                        persent = (int)(current_line / lines_count * 100);
                        if (persent % 10 == 0) Console.WriteLine("Чтение вершин из файла: " + persent + "%");
                    }
                    current_line++;

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

            //Console.Clear();
            Console.WriteLine("Сохранение вершин");
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

            using (StreamReader reader = new StreamReader(oldFilePath))
            {
                float current_line = 1;
                int persent = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    int current_persent = (int)(current_line / lines_count * 100);
                    if (current_persent > persent)
                    {
                        //Console.Clear();
                        persent = (int)(current_line / lines_count * 100);
                        if (persent % 10 == 0) Console.WriteLine("Чтение поверхностей из файла: " + persent + "%");
                    }
                    current_line++;

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
                                int n_ind = 0;
                                if (old_normals.Count > 0)
                                {
                                    n_ind = norm[old_normals[int.Parse(_f[2]) - 1]];
                                }

                                newline += " " + v_ind + "/" + t_ind + (old_normals.Count == 0 ? "" : "/" + n_ind); 
                            }
                            faceSectorFile.WriteLine(newline);
                            break;

                        case "g":       //сохранить в faceSector
                        case "s":
                        case "usemtl":
                            faceSectorFile.WriteLine(line);
                            break;
                        default:
                            verticesSector += line + "\n";
                            break;
                    }
                }
            }

            faceSectorFile.Close();
            StreamReader faceSectorFileReader = new StreamReader(tempFilePath);
            File.Delete(newFilePath);

            //Console.Clear();
            Console.WriteLine("Запись в новый файл");

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

                while((line = faceSectorFileReader.ReadLine()) != null)
                {
                    sw.WriteLine(line);
                }
                faceSectorFileReader.Close();

                sw.Close();
            }

            File.Delete(tempFilePath);
        }

        //функция для создания нового файла obj со смещенным центром в точку 0, 0, 0
        public static void NormalizeObjectCenter(string oldFilePath, string newFilePath, out vector3f newmax)
        {
            vector3f max = new vector3f(-1000, -1000, -1000);
            vector3f min = new vector3f(1000, 1000, 1000);
            string? line;
            using (TextReader reader = new StreamReader(oldFilePath))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Replace("  ", " ");
                    line = line.Trim();
                    string[] parts = line.Split(' ');

                    if (parts[0] == "v")
                    {
                        vector3f current = new vector3f(float.Parse(parts[1], CultureInfo.InvariantCulture), float.Parse(parts[2], CultureInfo.InvariantCulture), float.Parse(parts[3], CultureInfo.InvariantCulture));
                        if (current.x > max.x) max.x = current.x;
                        if (current.y > max.y) max.y = current.y;
                        if (current.z > max.z) max.z = current.z;
                        if (current.x < min.x) min.x = current.x;
                        if (current.y < min.y) min.y = current.y;
                        if (current.z < min.z) min.z = current.z;
                    }
                }
            }
            vector3f mean = (max + min) / 2;
            newmax = max - mean;
            File.Delete(newFilePath);
            using (StreamWriter sw = new StreamWriter(newFilePath))
            {
                using (TextReader reader = new StreamReader(oldFilePath))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.Replace("  ", " ");
                        line = line.Trim();
                        string[] parts = line.Split(' ');

                        switch(parts[0])
                        {
                            case "v":
                                vector3f output_vector = new vector3f(float.Parse(parts[1], CultureInfo.InvariantCulture), float.Parse(parts[2], CultureInfo.InvariantCulture), float.Parse(parts[3], CultureInfo.InvariantCulture));
                                output_vector -= mean;
                                string output_line = "v " +  output_vector.x.ToString("0.000000", CultureInfo.InvariantCulture) + " " + output_vector.y.ToString("0.000000", CultureInfo.InvariantCulture) + " " + output_vector.z.ToString("0.000000", CultureInfo.InvariantCulture);
                                sw.WriteLine(output_line);
                                break;
                            default:
                                sw.WriteLine(line);
                                break;
                        }
                    }
                }
            }
        }

        //функция для создания нового файла obj со смещенным центром в точку 0, 0, 0 и нормализованным размером
        public static void NormalizeObjectSizeOnX(string oldFilePath, string newFilePath, float max_width = 10)
        {
            string tempFilePath = Path.GetDirectoryName(oldFilePath) + "\\tempFile.obj";
            NormalizeObjectCenter(oldFilePath, tempFilePath, out vector3f newmax);
            File.Delete(newFilePath);
            string? line;
            float rel = max_width / newmax.x;
            using (StreamWriter sw = new StreamWriter(newFilePath))
            {
                using (TextReader reader = new StreamReader(tempFilePath))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.Replace("  ", " ");
                        line = line.Trim();
                        string[] parts = line.Split(' ');
                        switch (parts[0])
                        {
                            case "v":
                                vector3f output_vector = new vector3f(float.Parse(parts[1], CultureInfo.InvariantCulture), float.Parse(parts[2], CultureInfo.InvariantCulture), float.Parse(parts[3], CultureInfo.InvariantCulture));
                                string output_line = "v " + (rel * output_vector.x).ToString("0.000000", CultureInfo.InvariantCulture) + " " + (rel * output_vector.y).ToString("0.000000", CultureInfo.InvariantCulture) + " " + (rel * output_vector.z).ToString("0.000000", CultureInfo.InvariantCulture);
                                sw.WriteLine(output_line);
                                break;
                            default:
                                sw.WriteLine(line);
                                break;
                        }
                    }
                }
            }

            File.Delete(tempFilePath);
        }

        public static void RotateObject(string oldFilePath, string newFilePath, vector3f RotateVector) 
        {
            if (RotateVector == vector3f.Zero) return;
            File.Delete(newFilePath);
            string? line;
            matrix4f rotateMatrix = new matrix4f();
            rotateMatrix.Rotate(RotateVector.x, RotateVector.y, RotateVector.z);
            Matrix4 rotateMatrixOTK = rotateMatrix.ToOpenTK();

            Vector4 output_vector;
            string output_line;

            using (StreamWriter sw = new StreamWriter(newFilePath))
            {
                using (TextReader reader = new StreamReader(oldFilePath))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.Replace("  ", " ");
                        line = line.Trim();
                        string[] parts = line.Split(' ');
                        switch (parts[0])
                        {
                            case "v":
                                output_vector = new Vector4(float.Parse(parts[1], CultureInfo.InvariantCulture), float.Parse(parts[2], CultureInfo.InvariantCulture), float.Parse(parts[3], CultureInfo.InvariantCulture), 1.0f);
                                output_vector = output_vector * rotateMatrixOTK;

                                output_line = "v " + (output_vector.X).ToString("0.000000", CultureInfo.InvariantCulture) + " " + (output_vector.Y).ToString("0.000000", CultureInfo.InvariantCulture) + " " + (output_vector.Z).ToString("0.000000", CultureInfo.InvariantCulture);
                                sw.WriteLine(output_line);
                                break;
                            case "vn":
                                output_vector = new Vector4(float.Parse(parts[1], CultureInfo.InvariantCulture), float.Parse(parts[2], CultureInfo.InvariantCulture), float.Parse(parts[3], CultureInfo.InvariantCulture), 1.0f);
                                output_vector = output_vector * rotateMatrixOTK;

                                output_line = "v " + (output_vector.X).ToString("0.000000", CultureInfo.InvariantCulture) + " " + (output_vector.Y).ToString("0.000000", CultureInfo.InvariantCulture) + " " + (output_vector.Z).ToString("0.000000", CultureInfo.InvariantCulture);
                                sw.WriteLine(output_line);
                                break;
                            default:
                                sw.WriteLine(line);
                                break;
                        }
                    }
                }
            }
        }
    }
}
