using Assimp;
using game_2.Brain.Lights.LightStructures;
using OpenTK.Mathematics;
using System.Xml.Linq;

namespace game_2.Brain.NewAssimpFolder
{
    public class AObject
    {
        private List<AEntry> _entries;

        private Scene _scene;
        private string _modelFilePath;
        private string _modelDirectoryPath;

        public Pipeline _pipeline;

        public AObject(string ModelFilePath) 
        {
            _modelFilePath = ModelFilePath;
            InitScene();
        }

        private void InitScene()
        {
            //загрузка сцены из файла
            _scene = new Scene();
            _entries = new List<AEntry>();
            _pipeline = new Pipeline();


            if (!File.Exists(_modelFilePath))
                throw new Exception("Ошибка: не существует файл " + _modelFilePath);
            _modelDirectoryPath = Path.GetDirectoryName(_modelFilePath);

            DateTime start = DateTime.Now;

            using (var importer = new AssimpContext())
            {
                _scene = importer.ImportFile(
                    _modelFilePath,
                    PostProcessSteps.Triangulate |
                    PostProcessSteps.GenerateSmoothNormals |
                    PostProcessSteps.CalculateTangentSpace);
            }

            ProcessNodes(_scene.RootNode);

            DateTime end = DateTime.Now;

            Console.WriteLine("         Время загрузки:" + (end - start).TotalSeconds.ToString());
        }

        private void ProcessNodes(Node node)
        {
            for (int i = 0; i < node.MeshCount; i++)
            {
                //Console.WriteLine("Mesh " + i + " processing...");
                ProcessMesh(_scene.Meshes[node.MeshIndices[i]]);
            }

            for (int i = 0; i < node.ChildCount; i++)
            {
                ProcessNodes(node.Children[i]);
            }
        }

        private void ProcessMesh(Mesh mesh)
        {
            var vertices = new List<AVertex>();
            var indices = new List<int>();

            for (int i = 0; i < mesh.VertexCount; i++)
            {
                var packed = new AVertex();

                packed.Pos = new Vector3(mesh.Vertices[i].X, mesh.Vertices[i].Y, mesh.Vertices[i].Z);
                packed.Normal = new Vector3(mesh.Normals[i].X, mesh.Normals[i].Y, mesh.Normals[i].Z);
                if (mesh.HasTextureCoords(0))
                {
                    packed.Tex = new Vector2(mesh.TextureCoordinateChannels[0][i].X, mesh.TextureCoordinateChannels[0][i].Y);
                }
                else
                {
                    packed.Tex = new Vector2(0.0f, 0.0f);
                }
                packed.Tangent = new Vector3(mesh.Tangents[i].X, mesh.Tangents[i].Y, mesh.Tangents[i].Z);
                packed.Bitangent = new Vector3(mesh.BiTangents[i].X, mesh.BiTangents[i].Y, mesh.BiTangents[i].Z);

                vertices.Add(packed);
            }

            for (int i = 0; i < mesh.FaceCount; i++)
            {
                Face face = mesh.Faces[i];
                for (int j = 0; j < face.IndexCount; j++)
                {
                    indices.Add((ushort)face.Indices[j]);
                }
            }

            ModelTexturePaths texturesPaths = new ModelTexturePaths();
            float shininess = 1;

            AMaterial material;

            if (mesh.MaterialIndex >= 0)
            {
                // Textures
                Material input_material = _scene.Materials[mesh.MaterialIndex];
                texturesPaths = ProcessTextures(input_material.GetAllMaterialTextures());
                shininess = input_material.Shininess;                
            }

            material = AMaterial.Init(texturesPaths);
            material.SetSpecularPower(shininess);

            _entries.Add(new AEntry(vertices, indices, material));
        }

        private ModelTexturePaths ProcessTextures(TextureSlot[] allTextures)
        {
            ModelTexturePaths texturesPath = new ModelTexturePaths();

            foreach (TextureSlot slot in allTextures)
            {
                if(slot.FilePath != null)
                {
                    if (slot.TextureType == TextureType.Diffuse)
                    {
                        texturesPath._DiffusePath = new string(Path.Combine(_modelDirectoryPath, slot.FilePath));
                    }
                    else if (slot.TextureType == TextureType.Normals)
                    {
                        texturesPath._NormalPath = new string(Path.Combine(_modelDirectoryPath, slot.FilePath));
                    }
                    else if (slot.TextureType == TextureType.Specular)
                    {
                        texturesPath._SpecularPath = new string(Path.Combine(_modelDirectoryPath, slot.FilePath));
                    }
                }
            }
            return texturesPath;
        }

        public void Draw()
        {
            foreach (AEntry item in _entries) item.Draw(_pipeline.getWorld(), _pipeline.getWVP());
        }

        public void Draw(ShaderName shader, Matrix4 matrix)
        {
            foreach (AEntry item in _entries) item.Draw(shader, matrix);
        }

        public void Draw(ShaderName shader, Matrix4 wvp, Matrix4 light_wvp, Matrix4 world)
        {
            foreach (AEntry item in _entries) item.Draw(shader, wvp, light_wvp, world);
        }

        public void OnDelete()
        {
            _scene.Clear();
            foreach (AEntry item in _entries) item.OnDelete();
        }

        // УСТАНОВИТЬ
        public void SetScale(float scale)
        {
            _pipeline.SetScale(scale);
        }

        public void SetScale(float scaleX, float scaleY, float scaleZ)
        {
            _pipeline.SetScale(scaleX, scaleY, scaleZ);
        }

        public void SetAngle(float angleX, float angleY, float angleZ)
        {
            _pipeline.SetAngle(angleX, angleY, angleZ);
        }

        public void SetPosition(float PosX, float PosY, float PosZ)
        {
            _pipeline.SetPosition(PosX, PosY, PosZ);
        }

        // НЕМЕДЛЕННО ОБНОВИТЬ
        public void ExpandImmediately(float scaleX, float scaleY, float scaleZ)
        {
            _pipeline.SetScale(_pipeline.ScaleX + scaleX, _pipeline.ScaleY + scaleY, _pipeline.ScaleZ + scaleZ);
        }

        public void ExpandImmediately(float value)
        {
            _pipeline.SetScale(_pipeline.ScaleX + value, _pipeline.ScaleY + value, _pipeline.ScaleZ + value);
        }

        public void RotateImmediately(float angleX, float angleY, float angleZ)
        {
            _pipeline.SetAngle(_pipeline.AngleX + angleX, _pipeline.AngleY + angleY, _pipeline.AngleZ + angleZ);
        }

        public void MoveImmediately(float PosX, float PosY, float PosZ)
        {
            _pipeline.SetPosition(_pipeline.PosX + PosX, _pipeline.PosY + PosY, _pipeline.PosZ + PosZ);
        }

        // СКОРОСТЬ * ВРЕМЯ
        public void Rotate(float speedX, float speedY, float speedZ, float time)
        {
            _pipeline.Rotate(speedX, speedY, speedZ, time);
        }

        public void Move(float speedX, float speedY, float speedZ, float time)
        {
            _pipeline.Move(speedX, speedY, speedZ, time);
        }

        public void Expand(float speedX, float speedY, float speedZ, float time)
        {
            _pipeline.Expand(speedX, speedY, speedZ, time);
        }

        public void Expand(float speedVal, float time)
        {
            _pipeline.Expand(speedVal, time);
        }

        public void Reset()
        {
            _pipeline.Reset();
        }
    }
}
