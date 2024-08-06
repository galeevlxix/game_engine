using Assimp;
using OpenTK.Mathematics;

namespace game_2.Brain.NewAssimpFolder
{
    public class AScene
    {
        private List<AMesh> _entries;

        private Scene _scene;
        private string _modelFilePath;
        private string _modelDirectoryPath;

        

        public AScene(string modelFilePath)
        {
            _modelFilePath = modelFilePath;

            //загрузка сцены из файла
            _scene = new Scene();
            _entries = new List<AMesh>();
            

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
                    PostProcessSteps.CalculateTangentSpace |
                    PostProcessSteps.JoinIdenticalVertices |
                    PostProcessSteps.SortByPrimitiveType
                    );
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

            _entries.Add(new AMesh(vertices, indices, material));
        }

        private ModelTexturePaths ProcessTextures(TextureSlot[] allTextures)
        {
            ModelTexturePaths texturesPath = new ModelTexturePaths();

            foreach (TextureSlot slot in allTextures)
            {
                if (slot.FilePath != null)
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

        public void Draw(Shader shader, Pipeline _pipeline, Matrix4 view, Matrix4 pers)
        {
            // ввод матриц WVP и WORLD в шейдер
            shader.setValue("wvp", _pipeline.getWorld() * view * pers);
            shader.setValue("world", _pipeline.getWorld());

            // рисуем мэши объекта
            foreach (AMesh item in _entries) item.Draw();
        }

        public void Draw(Shader shader, Pipeline _pipeline)
        {
            // рисуем мэши объекта
            if (shader.name == ShaderName.SelectingShader)
            {
                int i = 0;
                shader.setValue("wvp", _pipeline.getWVP_SelectingShader());
                foreach (AMesh item in _entries)
                {
                    shader.setValue("gDrawIndex", i);
                    i++;
                    item.Draw();
                }
            }
            else
            {
                // ввод матриц WVP и WORLD в шейдер
                shader.setValue("wvp", _pipeline.getWVP());
                shader.setValue("world", _pipeline.getWorld());
                foreach (AMesh item in _entries)
                {
                    item.Draw();
                }
            }
        }

        public void OnDelete()
        {
            _scene.Clear();
            foreach (AMesh item in _entries) item.Dispose();
        }
    }
}
