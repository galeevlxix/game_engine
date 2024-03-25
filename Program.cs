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

            windowSettings.Size = new OpenTK.Mathematics.Vector2i(1920, 1080);

            windowSettings.Title = "Game";

            settings.IsMultiThreaded = false;

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
    }
}
