using UnityEngine;

public class metaballRender : MonoBehaviour
{
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public int vertexCount;
    public static int numberPoints = 512;
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        Mesh geometria = new Mesh();
        Vector3[] vertices = new Vector3[numberPoints + 1];
        int[] triangles = new int[numberPoints * 3];

        for (int i = 0; i < numberPoints; i++)
        {
            float angulo = ((360f / numberPoints) * i) * Mathf.Deg2Rad;

            vertices[i + 1] = new Vector3(Mathf.Cos(angulo), Mathf.Sin(angulo));
        }

        for (int i = 0; i < numberPoints; i++)
        {
            int t = i * 3;

            triangles[t] = 0;
            triangles[t + 1] = i + 1;
            triangles[t + 2] = (i + 1) % numberPoints + 1;
        }

        /*
        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(1, 0, 0);
        vertices[2] = new Vector3(0, 1, 0);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        */

        geometria.vertices = vertices;
        geometria.triangles = triangles;
        geometria.RecalculateNormals();
        geometria.RecalculateBounds();

        meshFilter.mesh = geometria;

        vertexCount = geometria.vertexCount;

        meshRenderer.material.SetFloat("_TotalVertices", vertexCount);

        meshRenderer.material.SetFloat("_PasoPorVertice", (360.0f * Mathf.Deg2Rad) / vertexCount);
    }
}
