using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RingMeshGenerator : MonoBehaviour
{
    public float innerRadius = 0.5f;
    public float outerRadius = 1.25f; // Increased outer radius for a thicker ring
    public int segments = 144; // Increased segment count for smoother edges and a more solid look

    void Start()
    {
        GenerateMesh();
    }

    void GenerateMesh()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[segments * 2];
        int[] triangles = new int[segments * 6];
        Vector2[] uv = new Vector2[vertices.Length];

        float angleStep = 2 * Mathf.PI / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep;
            float x = Mathf.Cos(angle);
            float y = Mathf.Sin(angle);

            vertices[i * 2] = new Vector3(x * innerRadius, y * innerRadius, 0);
            vertices[i * 2 + 1] = new Vector3(x * outerRadius, y * outerRadius, 0);

            uv[i * 2] = new Vector2((float)i / segments, 0);
            uv[i * 2 + 1] = new Vector2((float)i / segments, 1);

            int nextI = (i + 1) % segments;

            triangles[i * 6] = i * 2;
            triangles[i * 6 + 1] = nextI * 2;
            triangles[i * 6 + 2] = i * 2 + 1;

            triangles[i * 6 + 3] = nextI * 2;
            triangles[i * 6 + 4] = nextI * 2 + 1;
            triangles[i * 6 + 5] = i * 2 + 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }
}