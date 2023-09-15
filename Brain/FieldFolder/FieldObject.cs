using game_2.Brain.ObjectFolder;
using game_2.FileManagers;
using game_2.MathFolder;

namespace game_2.Brain.FieldFolder
{
    public class FieldObject : GameObj
    {
        public FieldObject(int Width, FieldType Type, float heightAmplitude, string texPath = "C:\\Users\\Lenovo\\source\\repos\\game_2\\Textures\\stone2.jpg") 
        {
            GetArraysOfVerticesAndIndices(Width, Type, heightAmplitude, out float[] vertices, out int[] indices);
            mesh = new Mesh(vertices, indices, texPath);
            pipeline = new Pipeline();
        }

        public enum FieldType
        {
            Flat,
            Crooked
        }

        private static void GetArraysOfVerticesAndIndices(int width, FieldType type, float heightAmplitude, out float[] vertices, out int[] indices)
        {
            Random random = new Random();
            List<List<vector3f>> cordList = new List<List<vector3f>>();

            // создали координаты вершин
            for (float i = -(float)width / 2;  i <= (float)width / 2; i++)
            {
                List<vector3f> rowTemp = new List<vector3f>();
                
                for (float j = -(float)width / 2; j <= (float)width / 2; j++)
                {
                    float height;

                    if (type == FieldType.Crooked)
                    {
                        height = random.NextSingle() * heightAmplitude;
                    }
                    else
                    {
                        height = 0;
                    }

                    vector3f vertTemp = new vector3f ( i, height, j );

                    rowTemp.Add(vertTemp);
                }
                cordList.Add(rowTemp);
            }

            // создали координаты текстур
            List<vector2f> textureCords = new List<vector2f>();

            textureCords.Add(new vector2f (  0,  0 ));
            textureCords.Add(new vector2f (  1,  0 ));
            textureCords.Add(new vector2f (  1,  1 ));
            textureCords.Add(new vector2f (  0,  1 ));

            // точки в примитивы с нормалями
            List<List<vector3f>> triangles = new List<List<vector3f>>();
            List<List<vector2f>> textriangles = new List<List<vector2f>>();
            List<vector3f> normals = new List<vector3f>();

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    List<vector3f> tempTrian1 = new List<vector3f>();
                    List<vector3f> tempTrian2 = new List<vector3f>();

                    List<vector2f> tempTexTrian1 = new List<vector2f>();
                    List<vector2f> tempTexTrian2 = new List<vector2f>();

                    tempTrian1.Add(cordList[i][j]);
                    tempTrian1.Add(cordList[i + 1][j]);
                    tempTrian1.Add(cordList[i + 1][j + 1]);

                    tempTexTrian1.Add(textureCords[0]);
                    tempTexTrian1.Add(textureCords[3]);
                    tempTexTrian1.Add(textureCords[2]);

                    tempTrian2.Add(cordList[i][j]);
                    tempTrian2.Add(cordList[i][j + 1]);
                    tempTrian2.Add(cordList[i + 1][j + 1]);

                    tempTexTrian2.Add(textureCords[0]);
                    tempTexTrian2.Add(textureCords[1]);
                    tempTexTrian2.Add(textureCords[2]);

                    ModelLoader.CalcNormals(tempTrian1[0], tempTrian1[2], tempTrian1[1], out vector3f normal1);
                    ModelLoader.CalcNormals(tempTrian2[0], tempTrian2[1], tempTrian2[2], out vector3f normal2);

                    triangles.Add(tempTrian1);
                    triangles.Add(tempTrian2);

                    textriangles.Add(tempTexTrian1);
                    textriangles.Add(tempTexTrian2);

                    normals.Add(normal1);
                    normals.Add(normal2);
                }
            }

            // Соединение 
            List<float> finalArray = new List<float>();
            List<int> Indices = new List<int>();
            int currentInd = 0;

            for (int i = 0; i < triangles.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    //cords
                    finalArray.Add(triangles[i][j].x);
                    finalArray.Add(triangles[i][j].y);
                    finalArray.Add(triangles[i][j].z);

                    //texture
                    finalArray.Add(textriangles[i][j].x);
                    finalArray.Add(textriangles[i][j].y);

                    //normals
                    finalArray.Add(normals[i].x);
                    finalArray.Add(normals[i].y);
                    finalArray.Add(normals[i].z);

                    //indices
                    Indices.Add(currentInd);
                    currentInd++;
                }
            }

            vertices = finalArray.ToArray();
            indices = Indices.ToArray();
        }
    }
}
