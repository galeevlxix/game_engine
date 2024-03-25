using game_2.Brain.Lights;
using game_2.MathFolder;
using System.Globalization;

namespace game_2.Brain.Compiler
{
    public static class ConsoleCompiler
    {
        private static string? line = "";
        private static bool isExecuting = false;
        private static string light_configuration_file = "..\\..\\..\\Files\\CompilerFiles\\light_configuration.txt";

        private static Dictionary<string, string> commands = new Dictionary<string, string>();

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
                string feedback = CompileLine(line);
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
                    if (commands.ContainsKey(key))
                    {
                        commands.Remove(key);
                    }
                    commands.Add(key, line);
                }

                line = "";
                isExecuting = false;
            }
        }
        private static string CompileLine(string? line)
        {
            line = line.Trim();
            line = line.Replace("  ", " ");
            string[] parts = line.Split(' ');
            if (parts.Length == 0) return "Пустая строка";

            switch (parts[0])
            {
                case "set":
                    return SetChoice(parts);
                case "get":
                    return GetChoice(parts);
                case "save":
                    return SaveChoice(parts);
                case "load":
                    return LoadChoice(parts);
                case "help":
                    return helper.PrintCommands(parts);
            }
            return "0# Неизвестное действие";
        }

        //GET
        private static string GetChoice(string[] parts)
        {
            switch (parts[1])
            {
                case "position":
                    return Camera.Pos.ToStr();
                case "target":
                    return Camera.Target.ToStr();
            }
            return "0# Неизвестное get действие";
        }

        //SAVE
        private static string SaveChoice(string[] parts)
        {
            switch (parts[1])
            {
                case "light":
                case "lightconfig":
                case "lightconfiguration":
                    using (StreamWriter sw = new StreamWriter(light_configuration_file))
                    {
                        foreach (string line in commands.Values) sw.WriteLine(line);
                        sw.Close();
                    }
                    return "Команды настройки света сохранены в файл";
            }
            return "0# Неизвестное save действие";
        }

        //LOAD
        private static string LoadChoice(string[] parts)
        {
            switch (parts[1])
            {
                case "light":
                case "lightconfig":
                case "lightconfiguration":
                    string? line;
                    using (StreamReader sr = new StreamReader(light_configuration_file))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            CompileLine(line);
                        }
                    }
                    return "Команды настройки света загружены из файла";
            }
            return "0# Неизвестное load действие";
        }

        //SET
        private static string SetChoice(string[] parts)
        {
            if (parts.Length == 1) return "0# Незаконченное set действие";
            switch (parts[1])
            {
                case "light":
                    return SetLightChoice(parts);
                case "object":
                case "camera":
                    return "0# Недоступно";
            }
            return "0# Неизвестное set действие";
        }

        private static string SetLightChoice(string[] parts)
        {
            if (parts.Length == 2) return "0# Незаконченное set light действие";

            switch (parts[2])
            {
                case "baselight":
                    return SetBaseLightChoice(parts);
                case "directionallight":
                    return SetDirectionalLightChoice(parts);
                case "specularlight":
                    return SetSpecularLightChoice(parts);
                case "pointlight":
                    if (int.TryParse(parts[3], out int result))
                    {
                        if (result >= 0 && result < LightningManager.PointlightsCount)
                            return SetPointLightChoice(parts, result);
                        else return "0# Индекс за пределами массива pointlights";
                    }
                    else return "0# Индекс pointlight должен быть числом";
                case "spotlight":
                    if (int.TryParse(parts[3], out int res))
                    {
                        if (res >= 0 && res < LightningManager.SpotlightsCount)
                            return SetSpotLightChoice(parts, res);
                        else return "0# Индекс за пределами массива spotlights";
                    }
                    else return "0# Индекс spotlight должен быть числом";
            }
            return "0# Неизвестное set light действие";
        }

        //SET -> BASE LIGHT
        private static string SetBaseLightChoice(string[] parts)
        {
            if (parts.Length == 3) return "0# Незаконченное set light действие";

            switch (parts[3])
            {
                case "color":
                    float red = float.Parse(parts[4], CultureInfo.InvariantCulture);
                    float green = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    float blue = float.Parse(parts[6], CultureInfo.InvariantCulture);
                    vector3f color = new vector3f(red, green, blue);
                    LightningManager.lightConfig.SetBaseLightColor(color);
                    return "1#1#1# Окружающий свет изменен на Color(" + red + ", " + green + ", " + blue + ")";
                case "intensity":
                    float intensity = float.Parse(parts[4], CultureInfo.InvariantCulture);
                    LightningManager.lightConfig.SetBaseLightIntensity(intensity);
                    return "1#1#2# Яркость окружающего света изменена на " + intensity;
            }
            return "0# Неизвестное set light действие";
        }

        //SET -> DIRECTIONAL LIGHT
        private static string SetDirectionalLightChoice(string[] parts)
        {
            if (parts.Length == 3) return "0# Незаконченное set light действие";
            switch (parts[3])
            {
                case "color":
                    float red = float.Parse(parts[4], CultureInfo.InvariantCulture);
                    float green = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    float blue = float.Parse(parts[6], CultureInfo.InvariantCulture);
                    vector3f color = new vector3f(red, green, blue);
                    LightningManager.lightConfig.SetDirectionalLightColor(color);
                    return "1#2#1# Напраленный свет изменен на Color(" + red + ", " + green + ", " + blue + ")";
                case "intensity":
                    float intensity = float.Parse(parts[4], CultureInfo.InvariantCulture);
                    LightningManager.lightConfig.SetDirectionalLightIntensity(intensity);
                    return "1#2#2# Яркость направленного света изменена на " + intensity;
                case "direction":
                    float x = float.Parse(parts[4], CultureInfo.InvariantCulture);
                    float y = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    float z = float.Parse(parts[6], CultureInfo.InvariantCulture);
                    vector3f dir = new vector3f(x, y, z);
                    LightningManager.lightConfig.SetDirectionalLightDirection(dir);
                    return "1#2#3# Направление света изменено на Direction(" + x + ", " + y + ", " + z + ")";

            }
            return "0# Неизвестное set light действие";
        }

        //SET -> SPECULAR LIGHT
        private static string SetSpecularLightChoice(string[] parts)
        {
            if (parts.Length == 3) return "0# Незаконченное set light действие";
            switch (parts[3])
            {
                case "power":
                    float power = float.Parse(parts[4], CultureInfo.InvariantCulture);
                    LightningManager.lightConfig.SetMatSpecularPower(power);
                    return "1#3#1# Сила отражения света изменена на " + power;
                case "intensity":
                    float intensity = float.Parse(parts[4], CultureInfo.InvariantCulture);
                    LightningManager.lightConfig.SetMatSpecularIntensity(intensity);
                    return "1#3#2# Интенсивность отражения света изменена на " + intensity;
            }
            return "0# Неизвестное set light действие";
        }

        //SET POINT LIGHT
        private static string SetPointLightChoice(string[] parts, int index)
        {
            if (parts.Length == 4) return "0# Незаконченное set light действие";
            switch (parts[4])
            {
                case "position":
                    float x = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    float y = float.Parse(parts[6], CultureInfo.InvariantCulture);
                    float z = float.Parse(parts[7], CultureInfo.InvariantCulture);
                    LightningManager.pointLights[index].Position = new vector3f(x, y, z);
                    return "1#4#1#" + index + "# Позиция точечного света " + index + " изменена на Position(" + x + ", " + y + ", " + z + ")";
                case "move":
                    float x1 = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    float y1 = float.Parse(parts[6], CultureInfo.InvariantCulture);
                    float z1 = float.Parse(parts[7], CultureInfo.InvariantCulture);
                    LightningManager.pointLights[index].Position += new vector3f(x1, y1, z1);
                    return "1#4#2#" + index + "# Позиция точечного света " + index + " смещена на +Position(" + x1 + ", " + y1 + ", " + z1 + ")";
                case "color":
                    float red = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    float green = float.Parse(parts[6], CultureInfo.InvariantCulture);
                    float blue = float.Parse(parts[7], CultureInfo.InvariantCulture);
                    LightningManager.pointLights[index].BaseLight.Color = new vector3f(red, green, blue);
                    return "1#4#3#" + index + "# Цвет точечного света " + index + " изменен на Color(" + red + ", " + green + ", " + blue + ")";
                case "intensity":
                    float intensity = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    LightningManager.pointLights[index].BaseLight.Intensity = intensity;
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

            return "0# Неизвестное set light действие";
        }

        //SET SPOT LIGHT
        private static string SetSpotLightChoice(string[] parts, int index)
        {
            if (parts.Length == 4) return "0# Незаконченное set light действие";
            switch (parts[4])
            {
                case "position":
                    float x = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    float y = float.Parse(parts[6], CultureInfo.InvariantCulture);
                    float z = float.Parse(parts[7], CultureInfo.InvariantCulture);
                    LightningManager.spotlights[index].PointLight.Position = new vector3f(x, y, z);
                    return "1#5#1#" + index + "# Позиция прожекторного света " + index + " изменена на Position(" + x + ", " + y + ", " + z + ")";
                case "move":
                    float x1 = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    float y1 = float.Parse(parts[6], CultureInfo.InvariantCulture);
                    float z1 = float.Parse(parts[7], CultureInfo.InvariantCulture);
                    LightningManager.spotlights[index].PointLight.Position += new vector3f(x1, y1, z1);
                    return "1#5#2#" + index + "# Позиция прожекторного света " + index + " смещена на +Position(" + x1 + ", " + y1 + ", " + z1 + ")";
                case "color":
                    float red = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    float green = float.Parse(parts[6], CultureInfo.InvariantCulture);
                    float blue = float.Parse(parts[7], CultureInfo.InvariantCulture);
                    LightningManager.spotlights[index].PointLight.BaseLight.Color = new vector3f(red, green, blue);
                    return "1#5#3#" + index + "# Цвет прожекторного света " + index + " изменен на Color(" + red + ", " + green + ", " + blue + ")";
                case "intensity":
                    float intensity = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    LightningManager.spotlights[index].PointLight.BaseLight.Intensity = intensity;
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
                    LightningManager.spotlights[index].Direction = dir;
                    return "1#5#8#" + index + "# Направление прожекторного света изменено на Direction(" + x2 + ", " + y2 + ", " + z2 + ")";
                case "cutoff":
                    float cutoff = float.Parse(parts[5], CultureInfo.InvariantCulture);
                    LightningManager.spotlights[index].Cutoff1 = cutoff;
                    return "1#5#9#" + index + "# Cutoff прожекторного света " + index + " изменен на " + cutoff;
            }

            return "0# Неизвестное set light действие";
        }
    }
}
