using game_2.Brain.Lights;
using game_2.MathFolder;
using System.Globalization;
using System.IO;
using System.Net.Mail;

namespace game_2.Brain.Compiler
{
    public static class ConsoleCompiler
    {
        private static string? line = "";
        private static bool isExecuting = false;
        private static string light_configuration_file = "..\\..\\..\\Files\\CompilerFiles\\log_config\\light_configuration.txt";
        private static string object_configuration_file = "..\\..\\..\\Files\\CompilerFiles\\log_config\\object_configuration.txt";

        private static Dictionary<string, string> commands = new Dictionary<string, string>();

        private static List<string> prohibited_to_delete = new List<string>()
        {
            "2_4", "2_5", "2_6"
        };
        private static int unique_num = 0;

        private static CompilerHelper helper = new CompilerHelper();
        public static void Run()
        {
            while (!isExecuting)
            {
                line = Console.ReadLine();
            }
        }

        public static void Execute()
        {
            if (line != "" && !isExecuting)
            {
                isExecuting = true;
                string feedback = ParseLine(line);
                Console.WriteLine(feedback);
                string[] parts = feedback.Split('#');

                //save commands to logs
                if (parts.Length > 1 && parts[0] != "0")
                {
                    string key = parts[0];
                    for (int i = 1; i < parts.Length - 1; i++)
                    {
                        key += "_" + parts[i];
                    }
                    
                    if(!prohibited_to_delete.Contains(parts[0] + "_" + parts[1]))
                    {
                        if (commands.ContainsKey(key))
                        {
                            commands.Remove(key);
                        }
                        commands.Add(key, line);
                    }
                    else
                    {
                        unique_num++;
                        commands.Add(key + "_" + unique_num, line);
                    }

                }

                line = "";
                isExecuting = false;
            }
        }

        private static string ParseLine(string? line)
        {
            line = line.Trim();
            line = line.Replace("  ", " ");
            string[] parts = line.Split(' ');
            if (parts.Length == 0) return "Пустая строка";

            switch (parts[0])
            {
                case "alter":
                    return AlterChoice(parts);
                case "get":
                    return GetChoice(parts);
                case "save":
                    return SaveChoice(parts);
                case "load":
                    return LoadChoice(parts);
                case "compress":
                    return CompressObjFile(parts[1], parts[2]);
                case "#":
                    return "";
                case "help":
                    return helper.PrintCommands(parts);
            }
            return "0# Неизвестное действие";
        }

        //GET
        private static string GetChoice(string[] parts)
        {
            if (parts.Length == 1) return "0# Незаконченное get действие";
            switch (parts[1])
            {
                case "camera":
                    return GetCameraChoice(parts);
                case "fps":
                    return FPSMeter.Int_FPS + " FPS";
                case "object":
                case "light":
                    return "0# Недоступно";
            }
            return "0# Неизвестное get действие";
        }

        private static string GetCameraChoice(string[] parts)
        {
            if (parts.Length == 2) return "0# Незаконченное get camera действие";

            switch (parts[2])
            {
                case "position":
                    return Camera.Pos.ToStr();
                case "target":
                    return (-Camera.Target).ToStr();
                case "up":
                    return Camera.Up.ToStr();
                case "persproj":
                    return GetPersProj(parts);
            }
            return "0# Неизвестное get camera действие";
        }

        private static string GetPersProj(string[] parts)
        {
            if (parts.Length == 3) return "0# Незаконченное get camera persproj действие";

            switch (parts[3])
            {
                case "fov":
                    return "FOV: " + mPersProj.GetFOV;
                case "width":
                    return "WIDTH: " + mPersProj.GetWidth;
                case "height":
                    return "HEIGHT: " + mPersProj.GetHeight;
                case "znear":
                    return "ZNEAR: " + mPersProj.GetZNear;
                case "zfar":
                    return "ZFAR: " + mPersProj.GetZFar;
            }

            return "0# Неизвестное get camera persproj действие";
        }

        //SAVE
        private static string SaveChoice(string[] parts)
        {
            switch (parts[1])
            {
                case "light":
                    using (StreamWriter sw = new StreamWriter(light_configuration_file))
                    {
                        foreach (KeyValuePair<string, string> item in commands)
                        {                            
                            if (item.Key.Split('_')[0] == "1") sw.WriteLine(item.Value);
                        }
                        sw.Close();
                    }
                    return "Команды настройки света сохранены в файл";
                case "object":
                    using (StreamWriter sw = new StreamWriter(object_configuration_file))
                    {
                        foreach (KeyValuePair<string, string> item in commands)
                        {
                            if (item.Key.Split('_')[0] == "2") sw.WriteLine(item.Value);
                        }
                        sw.Close();
                    }
                    return "Команды настройки объектов сохранены в файл";
                case "all":
                    return SaveChoice(new string[] { "save", "light" }) + "\n" + SaveChoice(new string[] { "save", "object" });
            }
            return "0# Неизвестное save действие";
        }

        //LOAD
        private static string LoadChoice(string[] parts)
        {
            switch (parts[1])
            {
                case "light":
                    string? line;
                    using (StreamReader sr = new StreamReader(light_configuration_file))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            ParseLine(line);
                        }
                    }
                    return "Команды настройки света загружены из файла";
                case "object":
                    string? line1;
                    using (StreamReader sr = new StreamReader(object_configuration_file))
                    {
                        while ((line1 = sr.ReadLine()) != null)
                        {
                            ParseLine(line1);
                        }
                    }
                    return "Команды настройки объектов загружены из файла";
                case "all":
                    return LoadChoice(new string[] { "save", "light" }) + "\n" + LoadChoice(new string[] { "save", "object" });
            }
            return "0# Неизвестное load действие";
        }

        //ALTER
        private static string AlterChoice(string[] parts)
        {
            if (parts.Length == 1) return "0# Незаконченное alter действие";
            switch (parts[1])
            {
                case "light":
                    return AlterLightChoice(parts);
                case "object":  //material, scale, angle, position
                    if (parts.Length == 2 || parts[2] == null || parts[2] == string.Empty) return "0# Незаконченное alter object действие -> Необходимо указать имя объекта";
                    if (ObjectArray.Count == 0) return "0# Массив Assimp-объектов пуст";
                    if (!ObjectArray.Exists(parts[2])) return "0# Объекта " + parts[2] + " не существует в массиве Assimp-объектов";
                    return AlterObjectChoice(parts, parts[2]);
                case "camera":  //position, target, 
                    return "0# Недоступно";
            }
            return "0# Неизвестное alter действие";
        }

        private static string AlterObjectChoice(string[] parts, string obj)
        {
            if (parts.Length == 3) return "0# Незаконченное alter object действие -> Необходимо указать изменяемый параметр и его значение";
            switch (parts[3])
            {
                //УСТАНОВИТЬ
                case "scale":
                    if (parts.Length == 5)
                    {
                        float value;
                        try
                        {
                            value = float.Parse(parts[4], CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            return "0# Не удается преобразовать в числовое значение";
                        }
                        ObjectArray.SetScale(obj, value, value, value);
                        return "2#1#" + obj + "# Масштаб объекта изменен на Scale(" + value + "," + value + "," + value + ")";
                    }
                    else if (parts.Length == 7)
                    {
                        float x, y, z;
                        try
                        {
                            x = float.Parse(parts[4], CultureInfo.InvariantCulture);
                            y = float.Parse(parts[5], CultureInfo.InvariantCulture);
                            z = float.Parse(parts[6], CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            return "0# Не удается преобразовать в числовое значение";
                        }
                        ObjectArray.SetScale(obj, x, y, z);
                        return "2#1#" + obj + "# Масштаб объекта изменен на Scale(" + x + "," + y + "," + z + ")";
                    }
                    else
                    {
                        return "0# Неизвестное alter object " + obj + " scale действие -> Необходимо ввести корректное значение";
                    }                    
                case "angle":
                    if (parts.Length == 7)
                    {
                        float x, y, z;
                        try
                        {
                            x = float.Parse(parts[4], CultureInfo.InvariantCulture);
                            y = float.Parse(parts[5], CultureInfo.InvariantCulture);
                            z = float.Parse(parts[6], CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            return "0# Не удается преобразовать в числовое значение";
                        }
                        ObjectArray.SetAngle(obj, x, y, z);
                        return "2#2#" + obj + "# Угол объекта изменен на Angle(" + x + "," + y + "," + z + ")";
                    }
                    else
                    {
                        return "0# Неизвестное alter object " + obj + " angle действие -> Необходимо ввести корректное значение";
                    }
                case "position":
                    if (parts.Length == 7)
                    {
                        float x, y, z;
                        try
                        {
                            x = float.Parse(parts[4], CultureInfo.InvariantCulture);
                            y = float.Parse(parts[5], CultureInfo.InvariantCulture);
                            z = float.Parse(parts[6], CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            return "0# Не удается преобразовать в числовое значение";
                        }
                        ObjectArray.SetPosition(obj, x, y, z);
                        return "2#3#" + obj + "# Позиция объекта изменена на Position(" + x + "," + y + "," + z + ")";
                    }
                    else
                    {
                        return "0# Неизвестное alter object " + obj + " position действие -> Необходимо ввести корректное значение";
                    }
                //НЕМЕДЛЕННО ОБНОВИТЬ
                case "expand":
                    if (parts.Length == 5)
                    {
                        float value;
                        try
                        {
                            value = float.Parse(parts[4], CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            return "0# Не удается преобразовать в числовое значение";
                        }
                        ObjectArray.ExpandImmediately(obj, value, value, value);
                        return "2#4#" + obj + "# Масштаб объекта увеличен на Expand(" + value + "," + value + "," + value + ")";
                    }
                    else if (parts.Length == 7)
                    {
                        float x, y, z;
                        try
                        {
                            x = float.Parse(parts[4], CultureInfo.InvariantCulture);
                            y = float.Parse(parts[5], CultureInfo.InvariantCulture);
                            z = float.Parse(parts[6], CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            return "0# Не удается преобразовать в числовое значение";
                        }
                        ObjectArray.ExpandImmediately(obj, x, y, z);
                        return "2#4#" + obj + "# Масштаб объекта увеличен на Expand(" + x + "," + y + "," + z + ")";
                    }
                    else
                    {
                        return "0# Неизвестное alter object " + obj + " expand действие -> Необходимо ввести корректное значение";
                    }
                case "rotate":
                    if (parts.Length == 7)
                    {
                        float x, y, z;
                        try
                        {
                            x = float.Parse(parts[4], CultureInfo.InvariantCulture);
                            y = float.Parse(parts[5], CultureInfo.InvariantCulture);
                            z = float.Parse(parts[6], CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            return "0# Не удается преобразовать в числовое значение";
                        }
                        ObjectArray.RotateImmediately(obj, x, y, z);
                        return "2#5#" + obj + "# Угол объекта увеличен на Rotate(" + x + "," + y + "," + z + ")";
                    }
                    else
                    {
                        return "0# Неизвестное alter object " + obj + " rotate действие -> Необходимо ввести корректное значение";
                    }
                case "move":
                    if (parts.Length == 7)
                    {
                        float x, y, z;
                        try
                        {
                            x = float.Parse(parts[4], CultureInfo.InvariantCulture);
                            y = float.Parse(parts[5], CultureInfo.InvariantCulture);
                            z = float.Parse(parts[6], CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            return "0# Не удается преобразовать в числовое значение";
                        }
                        ObjectArray.MoveImmediately(obj, x, y, z);
                        return "2#6#" + obj + "# Позиция объекта изменена на Move(" + x + "," + y + "," + z + ")";
                    }
                    else
                    {
                        return "0# Неизвестное alter object " + obj + " move действие -> Необходимо ввести корректное значение";
                    }

            }

            return "0# Неизвестное alter object действие";
        }
              

        private static string AlterLightChoice(string[] parts)
        {
            if (parts.Length == 2) return "0# Незаконченное alter light действие";

            switch (parts[2])
            {
                case "baselight":
                    return AlterBaseLightChoice(parts);
                case "directionallight":
                    return AlterDirectionalLightChoice(parts);
                case "pointlight":
                    if (int.TryParse(parts[3], out int result))
                    {
                        if (result >= 0 && result < LightningManager.PointlightsCount)
                            return AlterPointLightChoice(parts, result);
                        else return "0# Индекс за пределами массива pointlights";
                    }
                    else return "0# Индекс pointlight должен быть числом";
                case "spotlight":
                    if (int.TryParse(parts[3], out int res))
                    {
                        if (res >= 0 && res < LightningManager.SpotlightsCount)
                            return AlterSpotLightChoice(parts, res);
                        else return "0# Индекс за пределами массива spotlights";
                    }
                    else return "0# Индекс spotlight должен быть числом";
            }
            return "0# Неизвестное alter light действие";
        }

        //ALTER -> BASE LIGHT
        private static string AlterBaseLightChoice(string[] parts)
        {
            if (parts.Length == 3) return "0# Незаконченное alter light действие";

            switch (parts[3])
            {
                case "color":
                    float red;
                    float green;
                    float blue;
                    try
                    {
                        red = float.Parse(parts[4], CultureInfo.InvariantCulture);
                        green = float.Parse(parts[5], CultureInfo.InvariantCulture);
                        blue = float.Parse(parts[6], CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        return "0# Не удается преобразовать в числовое значение";
                    }
                    vector3f color = new vector3f(red, green, blue);
                    LightningManager.lightConfig.SetBaseLightColor(color);
                    return "1#1#1# Окружающий свет изменен на Color(" + red + ", " + green + ", " + blue + ")";
                case "intensity":
                    float intensity = float.Parse(parts[4], CultureInfo.InvariantCulture);
                    LightningManager.lightConfig.SetBaseLightIntensity(intensity);
                    return "1#1#2# Яркость окружающего света изменена на " + intensity;
            }
            return "0# Неизвестное alter light действие";
        }

        //ALTER -> DIRECTIONAL LIGHT
        private static string AlterDirectionalLightChoice(string[] parts)
        {
            if (parts.Length == 3) return "0# Незаконченное alter light действие";
            switch (parts[3])
            {
                case "color":
                    float red;
                    float green;
                    float blue;
                    try
                    {
                        red = float.Parse(parts[4], CultureInfo.InvariantCulture);
                        green = float.Parse(parts[5], CultureInfo.InvariantCulture);
                        blue = float.Parse(parts[6], CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        return "0# Не удается преобразовать в числовое значение";
                    }
                    vector3f color = new vector3f(red, green, blue);
                    LightningManager.lightConfig.SetDirectionalLightColor(color);
                    return "1#2#1# Напраленный свет изменен на Color(" + red + ", " + green + ", " + blue + ")";
                case "intensity":
                    float intensity = float.Parse(parts[4], CultureInfo.InvariantCulture);
                    LightningManager.lightConfig.SetDirectionalLightIntensity(intensity);
                    return "1#2#2# Яркость направленного света изменена на " + intensity;
                case "direction":
                    float x;
                    float y;
                    float z;
                    try
                    {
                        x = float.Parse(parts[4], CultureInfo.InvariantCulture);
                        y = float.Parse(parts[5], CultureInfo.InvariantCulture);
                        z = float.Parse(parts[6], CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        return "0# Не удается преобразовать в числовое значение";
                    }
                    vector3f dir = new vector3f(x, y, z);
                    LightningManager.lightConfig.SetDirectionalLightDirection(dir);
                    return "1#2#3# Направление света изменено на Direction(" + x + ", " + y + ", " + z + ")";

            }
            return "0# Неизвестное alter light действие";
        }

        //ALTER POINT LIGHT
        private static string AlterPointLightChoice(string[] parts, int index)
        {
            if (parts.Length == 4) return "0# Незаконченное alter light действие";
            switch (parts[4])
            {
                case "position":
                    float x = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    float y = float.Parse(parts[6], CultureInfo.InvariantCulture);
                    float z = float.Parse(parts[7], CultureInfo.InvariantCulture);
                    LightningManager.pointLights[index].SetPosition(x, y, z);
                    return "1#4#1#" + index + "# Позиция точечного света " + index + " изменена на Position(" + x + ", " + y + ", " + z + ")";
                case "move":
                    float x1 = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    float y1 = float.Parse(parts[6], CultureInfo.InvariantCulture);
                    float z1 = float.Parse(parts[7], CultureInfo.InvariantCulture);
                    LightningManager.pointLights[index].Move(x1, y1, z1);
                    return "1#4#2#" + index + "# Позиция точечного света " + index + " смещена на +Position(" + x1 + ", " + y1 + ", " + z1 + ")";
                case "color":
                    float red = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    float green = float.Parse(parts[6], CultureInfo.InvariantCulture);
                    float blue = float.Parse(parts[7], CultureInfo.InvariantCulture);
                    LightningManager.pointLights[index].SetColor(red, green, blue);
                    return "1#4#3#" + index + "# Цвет точечного света " + index + " изменен на Color(" + red + ", " + green + ", " + blue + ")";
                case "intensity":
                    float intensity = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    LightningManager.pointLights[index].SetIntensity(intensity);
                    return "1#4#4#" + index + "# Интенсивность точечного света " + index + " изменена на " + intensity;
                case "constant":
                    float constant = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    LightningManager.pointLights[index].Attenuation.Constant = constant;
                    return "1#4#5#" + index + "# Постоянное затухание точечного света " + index + " изменена на " + constant;
                case "linear":
                    float linear = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    LightningManager.pointLights[index].Attenuation.Linear = linear;
                    return "1#4#6#" + index + "# Линейное затухание точечного света " + index + " изменена на " + linear;
                case "exp":
                    float exp = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    LightningManager.pointLights[index].Attenuation.Exp = exp;
                    return "1#4#7#" + index + "# Экспоненциальное затухание точечного света " + index + " изменена на " + exp;
            }

            return "0# Неизвестное alter light действие";
        }

        //SET SPOT LIGHT
        private static string AlterSpotLightChoice(string[] parts, int index)
        {
            if (parts.Length == 4) return "0# Незаконченное alter light действие";
            switch (parts[4])
            {
                case "position":
                    float x = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    float y = float.Parse(parts[6], CultureInfo.InvariantCulture);
                    float z = float.Parse(parts[7], CultureInfo.InvariantCulture);
                    LightningManager.spotlights[index].PointLight.SetPosition(x, y, z);
                    return "1#5#1#" + index + "# Позиция прожекторного света " + index + " изменена на Position(" + x + ", " + y + ", " + z + ")";
                case "move":
                    float x1 = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    float y1 = float.Parse(parts[6], CultureInfo.InvariantCulture);
                    float z1 = float.Parse(parts[7], CultureInfo.InvariantCulture);
                    LightningManager.spotlights[index].PointLight.Move(x1, y1, z1);
                    return "1#5#2#" + index + "# Позиция прожекторного света " + index + " смещена на +Position(" + x1 + ", " + y1 + ", " + z1 + ")";
                case "color":
                    float red = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    float green = float.Parse(parts[6], CultureInfo.InvariantCulture);
                    float blue = float.Parse(parts[7], CultureInfo.InvariantCulture);
                    LightningManager.spotlights[index].PointLight.SetColor(red, green, blue);
                    return "1#5#3#" + index + "# Цвет прожекторного света " + index + " изменен на Color(" + red + ", " + green + ", " + blue + ")";
                case "intensity":
                    float intensity = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    LightningManager.spotlights[index].PointLight.SetIntensity(intensity);
                    return "1#5#4#" + index + "# Интенсивность прожекторного света " + index + " изменена на " + intensity;
                case "constant":
                    float constant = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    LightningManager.spotlights[index].PointLight.Attenuation.Constant = constant;
                    return "1#5#5#" + index + "# Постоянное затухание прожекторного света " + index + " изменена на " + constant;
                case "linear":
                    float linear = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    LightningManager.spotlights[index].PointLight.Attenuation.Linear = linear;
                    return "1#5#6#" + index + "# Линейное затухание прожекторного света " + index + " изменена на " + linear;
                case "exp":
                    float exp = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    LightningManager.spotlights[index].PointLight.Attenuation.Exp = exp;
                    return "1#5#7#" + index + "# Экспоненциальное затухание прожекторного света " + index + " изменена на " + exp;
                case "direction":
                    float x2 = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    float y2 = float.Parse(parts[6], CultureInfo.InvariantCulture);
                    float z2 = float.Parse(parts[7], CultureInfo.InvariantCulture);
                    vector3f dir = new vector3f(x2, y2, z2);
                    dir.Normalize();
                    LightningManager.spotlights[index].Direction = dir;
                    return "1#5#8#" + index + "# Направление прожекторного света изменено на Direction(" + x2 + ", " + y2 + ", " + z2 + ")";
                case "cutoff":
                    float cutoff = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    LightningManager.spotlights[index].Cutoff1 = cutoff;
                    return "1#5#9#" + index + "# Cutoff прожекторного света " + index + " изменен на " + cutoff;
            }

            return "0# Неизвестное alter light действие";
        }

        //функция для создания нового скомпрессированного файла obj. UPD: Больше не используется.
        private static string CompressObjFile(string oldFilePath, string newFilePath)
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

            return "Файл размером " + (new FileInfo(oldFilePath).Length / 1024).ToString()
                + "KB сжат в файл размером " + (new FileInfo(newFilePath).Length / 1024).ToString() + "KB";


        }
    }
}
