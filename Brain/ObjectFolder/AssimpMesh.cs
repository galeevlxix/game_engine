using game_2.MathFolder;
using System.Numerics;
using color_texture_unit = OpenTK.Graphics.OpenGL.TextureTarget;
using Assimp.Unmanaged;
using Assimp;
using OpenTK.Graphics.OpenGL;
using game_2.Brain.AimFolder;
using System.ComponentModel.DataAnnotations;
using Assimp.Configs;

namespace game_2.Brain.ObjectFolder
{
    public class AssimpMesh
    {
        private MeshEntry[] _Entries;
        private Texture[] _Textures;

        public AssimpMesh()
        {

        }

        ~AssimpMesh()
        {

        }

        public bool LoadMesh(string file_name)
        {
            Clear();

            bool ret;

            AssimpContext importer = new AssimpContext();

            if (!importer.IsImportFormatSupported(Path.GetExtension(file_name)))
            {
                Console.WriteLine("File format is not supprting");
            }

            var postProcessFlags = 
                PostProcessSteps.Triangulate | 
                PostProcessSteps.GenerateSmoothNormals |
                PostProcessSteps.FlipUVs |
                PostProcessSteps.CalculateTangentSpace;

            Scene scene = importer.ImportFile(file_name, postProcessFlags);

            ret = InitFromScene(ref scene, file_name);

            return ret;
        }

        private bool InitFromScene(ref Scene pScene, string file_name)
        {
            _Entries = new MeshEntry[pScene.MeshCount];
            _Textures = new Texture[pScene.TextureCount];

            //AiMesh[] aiMeshes = (AiMesh[])System.Runtime.InteropServices.Marshal.PtrToStructure(pScene.Meshes, typeof(AiMesh[]));

            for (int i = 0; i < _Entries.Length; i++)
            {
                Assimp.Mesh mesh = pScene.Meshes[i];

                InitMesh(i, mesh);
            }

            return InitMaterials(pScene, file_name);
        }

        private void InitMesh(int Index, Assimp.Mesh pMesh)
        {
            _Entries[Index]._materialIndex = pMesh.MaterialIndex;

            List<Vertex> Vertices = new List<Vertex>();
            List<int> Indices = new List<int>();

            Vector3D Zero3D = new Vector3D(0, 0, 0);

            for (int i = 0; i < pMesh.VertexCount; i++)
            {
                Vector3D pPos = pMesh.Vertices[i];
                Vector3D pNormal = pMesh.Normals[i];
                Vector3D pTexCoord = pMesh.HasTextureCoords(0) ? pMesh.TextureCoordinateChannels[0][i] : Zero3D;
                Vector3D pTangent = pMesh.Tangents[i];

                Vertex v = new Vertex(
                    new vector3f(pPos.X, pPos.Y, pPos.Z),
                    new vector2f(pTexCoord.X, pTexCoord.Y),
                    new vector3f(pNormal.X, pNormal.Y, pNormal.Z),
                    new vector3f(pTangent.X, pTangent.Y, pTangent.Z));

                Vertices.Add(v);
            }

            for (int i = 0; i < pMesh.FaceCount; i++)
            {
                Face face = pMesh.Faces[i];

                if (face.IndexCount != 3)
                {
                    Console.WriteLine("Неправильное кол-во индексов фейса");
                    return;
                }

                Indices.Add(face.Indices[0]);
                Indices.Add(face.Indices[1]);
                Indices.Add(face.Indices[2]);
            }

            _Entries[Index].Init(Vertices.ToArray(), Indices.ToArray());
        }

        private bool InitMaterials(Scene pScene, string file_name)
        {
            for (int i = 0; i < pScene.Materials.Count; i++)
            {
                Material pMaterial = pScene.Materials[i];

                _Textures[i] = null;

                if(pMaterial.GetMaterialTextureCount(TextureType.Diffuse) > 0)
                {
                    string path;

                    pMaterial.GetMaterialTexture(TextureType.Diffuse, 0, out TextureSlot texture);

                    _Textures[i] = Texture.Load(texture.FilePath);
                }

            }

            return true;
        }

        private void Clear()
        {
            for (int i = 0; i < _Textures.Length; i++)
            {
                _Textures[i].Dispose();
            }
        }

        public void Render()
        {
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.EnableVertexAttribArray(3);

            for (int i = 0; i < _Entries.Length; i++)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, _Entries[i]._VB);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.Size, 0);
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vertex.Size, 3 * sizeof(float));
                GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, Vertex.Size, 5 * sizeof(float));
                GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, Vertex.Size, 8 * sizeof(float));

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _Entries[i]._IB);
                int MaterialIndex = _Entries[i]._materialIndex;

                if(MaterialIndex < _Textures.Length && _Textures[MaterialIndex] != null)
                {
                    _Textures[MaterialIndex].Use(TextureUnit.Texture0);
                }

                GL.DrawElements(BeginMode.Triangles, _Entries[i]._numIndices, DrawElementsType.UnsignedInt, 0);
            }

            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);
            GL.DisableVertexAttribArray(3);
        }
    }
}
