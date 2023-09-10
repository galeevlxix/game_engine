using OpenTK.Mathematics;
using Assimp;

namespace game_2.Brain.AssimpFolder
{
    public class AssimpMesh : IDisposable
    {
        private Scene scene;
        public List<MeshEntry> meshes { get; }
        public MeshEntry FirstMesh => meshes[0];
        public string PathModel;

        public AssimpMesh(string pathModel, bool flipuvs)
        {
            if (!File.Exists(pathModel)) 
                throw new Exception("error: file not exists " +  pathModel);

            PathModel = Path.GetDirectoryName(pathModel);

            scene = new Scene();
            meshes = new List<MeshEntry>();

            using(var importer = new AssimpContext())
            {
                if (flipuvs)
                    scene = importer.ImportFile(
                        pathModel,
                        PostProcessSteps.Triangulate |
                        PostProcessSteps.GenerateSmoothNormals |
                        PostProcessSteps.FlipUVs |
                        PostProcessSteps.CalculateTangentSpace);
                else
                    scene = importer.ImportFile(
                        pathModel,
                        PostProcessSteps.Triangulate |
                        PostProcessSteps.GenerateSmoothNormals |
                        PostProcessSteps.CalculateTangentSpace);
            }

            ProcessNodes(scene.RootNode);
        }

        private void ProcessNodes(Node node)
        {
            for (int i = 0; i < node.MeshCount; i++)
            {
                ProcessMesh(scene.Meshes[node.MeshIndices[i]]);
            }

            for (int i = 0; i < node.ChildCount; i++)
            {
                ProcessNodes(node.Children[i]);
            }
        }

        private void ProcessMesh(Mesh mesh)
        {
            var vertices = new List<Vertex>();
            var indices = new List<int>();

            for (int i = 0; i < mesh.VertexCount; i++)
            {
                var packed = new Vertex();

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
                // Texturas
                Material material = scene.Materials[mesh.MaterialIndex];
                texturesPaths = ProcessTextures(material.GetAllMaterialTextures());
            }

            meshes.Add(new MeshEntry(vertices, indices, texturesPaths));
        }

        private ModelTexturePaths ProcessTextures(TextureSlot[] slot)
        {
            ModelTexturePaths texturesPath = new ModelTexturePaths();

            foreach (var item in slot)
            {
                if (item.FilePath != null)
                {
                    if (item.TextureType == TextureType.Diffuse)
                    {
                        texturesPath._DiffusePath = new string(Path.Combine(PathModel, item.FilePath));
                    }
                    else if (item.TextureType == TextureType.Specular)
                    {
                        texturesPath._SpecularPath = new string(Path.Combine(PathModel, item.FilePath));
                    }
                    else if (item.TextureType == TextureType.Normals)
                    {
                        texturesPath._NormalPath = new string(Path.Combine(PathModel, item.FilePath));
                    }
                    else if (item.TextureType == TextureType.Height)
                    {
                        texturesPath._HeightPath = new string(Path.Combine(PathModel, item.FilePath));
                    }
                    else if (item.TextureType == TextureType.Metalness)
                    {
                        texturesPath._MetallicPath = new string(Path.Combine(PathModel, item.FilePath));
                    }
                    else if (item.TextureType == TextureType.Roughness)
                    {
                        texturesPath._RoughnnesPath = new string(Path.Combine(PathModel, item.FilePath));
                    }
                    else if (item.TextureType == TextureType.Lightmap)
                    {
                        texturesPath._LightMap = new string(Path.Combine(PathModel, item.FilePath));
                    }
                    else if (item.TextureType == TextureType.Emissive)
                    {
                        texturesPath._EmissivePath = new string(Path.Combine(PathModel, item.FilePath));
                    }
                    else if (item.TextureType == TextureType.AmbientOcclusion)
                    {
                        texturesPath._AmbientOcclusionPath = new string(Path.Combine(PathModel, item.FilePath));
                    }
                }
            }

            return texturesPath;
        }

        public void Dispose()
        {
            scene.Clear();
        }
    }
}
