using UnityEngine;

public class metaballRender : MonoBehaviour
{
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private int vertexCount;
    private static int numberPoints = 32;
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        Mesh geometria = new Mesh();
        Vector3[] vertices = new Vector3[numberPoints];

        for (int i = 0; i < numberPoints; i++)
        {
            float angulo = ((360f / numberPoints + 2) * i) * Mathf.Deg2Rad;
        }

        geometria.vertices = vertices;
        meshFilter.mesh = geometria;

        vertexCount = geometria.vertexCount;

        meshRenderer.material.SetFloat("_TotalVertices", vertexCount);

        meshRenderer.material.SetFloat("_PasoPorVertice", (360.0f * Mathf.Deg2Rad) / vertexCount);
    }
}
