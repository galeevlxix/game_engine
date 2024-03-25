using Assimp;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_2.Brain.NewAssimpFolder
{
    public class AObject
    {
        private List<AEntry> _entries;

        private Scene _scene;
        private string _modelFilePath;
        private string _modelDirectoryPath;

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
            
            if (!File.Exists(_modelFilePath))
                throw new Exception("Ошибка: не существует файл " + _modelFilePath);
            _modelDirectoryPath = Path.GetDirectoryName(_modelFilePath);

            using (var importer = new AssimpContext())
            {
                _scene = importer.ImportFile(
                    _modelFilePath,
                    PostProcessSteps.Triangulate |
                    PostProcessSteps.GenerateSmoothNormals |
                    PostProcessSteps.CalculateTangentSpace);
            }

            ProcessNodes(_scene.RootNode);
        }

        private void ProcessNodes(Node node)
        {
            for (int i = 0; i < node.MeshCount; i++)
            {
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

            if (mesh.MaterialIndex >= 0)
            {
                // Textures
                Material material = _scene.Materials[mesh.MaterialIndex];
                texturesPaths = ProcessTextures(material.GetAllMaterialTextures());
            }

            _entries.Add(new AEntry(vertices, indices, texturesPaths));


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
                }
            }
            return texturesPath;
        }

        public void Draw()
        {
            foreach (AEntry item in _entries) item.Draw();
        }

        public void OnDelete()
        {
            _scene.Clear();
            foreach (AEntry item in _entries) item.OnDelete();
        }

        // Pipeline work
        public void SetScale(float scale)
        {
            foreach (AEntry item in _entries) item.pipeline.SetScale(scale);
        }

        public void SetScale(float scaleX, float scaleY, float scaleZ)
        {
            foreach (AEntry item in _entries) item.pipeline.SetScale(scaleX, scaleY, scaleZ);
        }

        public void SetAngle(float angleX, float angleY, float angleZ)
        {
            foreach (AEntry item in _entries) item.pipeline.SetScale(angleX, angleY, angleZ);
        }

        public void SetPosition(float PosX, float PosY, float PosZ)
        {
            foreach (AEntry item in _entries) item.pipeline.SetPosition(PosX, PosY, PosZ);
        }

        public void Rotate(float speedX, float speedY, float speedZ, float time)
        {
            foreach (AEntry item in _entries) item.pipeline.Rotate(speedX, speedY, speedZ, time);
        }

        public void Move(float speedX, float speedY, float speedZ, float time)
        {
            foreach (AEntry item in _entries) item.pipeline.Move(speedX, speedY, speedZ, time);
        }

        public void Expand(float speedX, float speedY, float speedZ, float time)
        {
            foreach (AEntry item in _entries) item.pipeline.Expand(speedX, speedY, speedZ, time);
        }

        public void Reset()
        {
            foreach (AEntry item in _entries) item.pipeline.Reset();
        }
    }
}
