using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_2.Brain
{
    public static class ObjectCreator
    {
        private static string modelsFolder = "";
        private static string picturesFolder = "";

        public static void Create(string configFile = "..\\..\\..\\Files\\ObjectsProperties\\models.txt")
        {
            using (StreamReader sr = new StreamReader(configFile))
            {
                string? line;
                string currentObjectName = string.Empty;
                int sculptureNumber = -1;
                bool skip = false;

                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim().Replace("  ", " ");

                    if (skip)
                    {
                        if (line == "*/") skip = false;
                        else continue;
                    }

                    string[] parts = line.Split(' ');

                    switch (parts[0])
                    {
                        case "/*":
                            skip = true;
                            break;
                        case "mf":
                            modelsFolder = parts[1];
                            break;
                        case "pf":
                            picturesFolder = parts[1];
                            break;
                        case "object":
                            currentObjectName = string.Empty;
                            for (int i = 1; i < parts.Length; i++)
                            {
                                currentObjectName += parts[i];
                            }
                            break;
                        case "path":
                            if (currentObjectName != string.Empty)
                            {
                                ObjectArray.Add(currentObjectName, modelsFolder + parts[1]);
                            }
                            break;
                        case "angle":
                            if (CheckObject(currentObjectName))
                            {
                                ObjectArray.SetAngle(
                                    currentObjectName,
                                    float.Parse(parts[1], CultureInfo.InvariantCulture),
                                    float.Parse(parts[2], CultureInfo.InvariantCulture),
                                    float.Parse(parts[3], CultureInfo.InvariantCulture));

                                if (ObjectArray.IsSculpture(currentObjectName))
                                {
                                    ObjectArray.SculptureAngles.Add(
                                        new MathFolder.vector3f(
                                        float.Parse(parts[1], CultureInfo.InvariantCulture),
                                        float.Parse(parts[2], CultureInfo.InvariantCulture),
                                        float.Parse(parts[3], CultureInfo.InvariantCulture)));
                                }
                            }
                            break;
                        case "position":
                            if (CheckObject(currentObjectName))
                            {
                                ObjectArray.SetPosition(
                                    currentObjectName,
                                    float.Parse(parts[1], CultureInfo.InvariantCulture),
                                    float.Parse(parts[2], CultureInfo.InvariantCulture),
                                    float.Parse(parts[3], CultureInfo.InvariantCulture));
                                
                                if (ObjectArray.IsSculpture(currentObjectName))
                                {
                                    ObjectArray.SculpturePositions.Add(
                                        new MathFolder.vector3f(
                                        float.Parse(parts[1], CultureInfo.InvariantCulture),
                                        float.Parse(parts[2], CultureInfo.InvariantCulture),
                                        float.Parse(parts[3], CultureInfo.InvariantCulture)));
                                }
                            }
                            break;
                        case "scale":
                            if (CheckObject(currentObjectName))
                            {
                                if (parts.Length == 4)
                                {
                                    ObjectArray.SetScale(
                                        currentObjectName,
                                        float.Parse(parts[1], CultureInfo.InvariantCulture),
                                        float.Parse(parts[2], CultureInfo.InvariantCulture),
                                        float.Parse(parts[3], CultureInfo.InvariantCulture));
                                }
                                else
                                {
                                    ObjectArray.SetScale(
                                    currentObjectName,
                                    float.Parse(parts[1], CultureInfo.InvariantCulture));
                                    
                                    if (ObjectArray.IsSculpture(currentObjectName))
                                    {
                                        ObjectArray.SculptureScales.Add(
                                            float.Parse(parts[1], CultureInfo.InvariantCulture));
                                    }
                                }
                            }
                            break;
                        case "isSculpture":
                            if (CheckObject(currentObjectName))
                            {
                                ObjectArray.MakeSculpture(currentObjectName);
                            }
                            break;
                        case "isPhysical":
                            if (CheckObject(currentObjectName))
                            {
                                ObjectArray.MakePhysical(currentObjectName);
                            }
                            break;
                        case "picture":
                            if (CheckObject(currentObjectName))
                            {
                                ObjectArray.AddPicture(currentObjectName, picturesFolder + parts[1]);
                            }
                            break;
                        case "description":
                            string? description_line;
                            while((description_line = sr.ReadLine()) != "#description")
                            {
                                ObjectArray.AddDescription(currentObjectName, description_line);
                            }
                            break;
                    }
                }
            }
        }

        private static bool CheckObject(string objectName)
        {
            return
                objectName != string.Empty &&
                objectName != null &&
                ObjectArray.Exists(objectName);
        }
    }
}
