using UnityEngine;

namespace UnityStandardAssets.Water
{
    public class MeshContainer
    {
        public readonly Mesh mesh;
        public readonly Vector3[] vertices;
        public readonly Vector3[] normals;


        public MeshContainer(Mesh m)
        {
            mesh = m;
            vertices = m.vertices;
            normals = m.normals;
        }


        public void Update()
        {
            mesh.vertices = vertices;
            mesh.normals = normals;
        }
    }
}