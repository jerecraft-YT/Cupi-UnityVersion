using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LineData
{
    //datos de la linea
    public int uniqueIndex;
    public float lineWidth;
    public Vector3[] points;
    public bool updateLine;
    public int startVertex;
    public int startTriangle;
    public Transform lineOrigin;
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MyLineRenderer : MonoBehaviour
{
    public static MyLineRenderer instance;

    public List<LineData> lines = new();

    private Mesh mesh;

    public List<Vector3> vertices = new();
    public List<int> triangles = new();

    public bool debugDraw;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance);
            return;
        }

        instance = this;

        mesh = new Mesh();

        //ampliamos el limite de vertices
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        GetComponent<MeshFilter>().mesh = mesh;

        ClearMesh();

    }

    private void Update()
    {
        foreach (var line in lines)
        {
            if (line.updateLine)
            {
                UpdateMesh(line);
            }
        }
    }
    private void UpdateMesh(LineData line)
    {
        MoveOrGenerateVertices(line);
        mesh.SetVertices(vertices);
        mesh.RecalculateBounds();
        line.updateLine = false;
    }

    private void ClearMesh()
    {
        vertices.Clear();
        triangles.Clear();
        mesh.Clear();
    }

    public void AddLine(LineData line ,int numberPoints,Transform lineOrigin, int uniqueIndex = -1 , float with = 1.0f)
    {
        line.points = new Vector3[numberPoints];
        line.lineWidth = with;
        line.uniqueIndex = uniqueIndex;
        line.lineOrigin = lineOrigin;

        //nos define el inicio de los vertices para cambiar de linea a linea
        line.startVertex = vertices.Count;
        line.startTriangle = triangles.Count;

        MoveOrGenerateVertices(line, true);

        GenerateTriangles(line, line.startVertex);

        lines.Add(line);

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateBounds();
    }

    public void DeleteLine(LineData line)
    {
        int vertexRemove = line.points.Length * 2;
        int trianglesRemove = (line.points.Length - 1) * 6;

        vertices.RemoveRange(line.startVertex, vertexRemove);
        triangles.RemoveRange(line.startTriangle, trianglesRemove);

        SortLines(lines.IndexOf(line), vertexRemove, trianglesRemove);

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateBounds();
    }

    private void SortLines(int lineIndex, int vertexRemove, int trianglesRemove)
    {
        for (int i = lineIndex + 1; i < lines.Count - lineIndex; i++)
        {
            lines[i].startVertex -= vertexRemove;
            lines[i].startTriangle -= trianglesRemove;
        }
    }

    private void MoveOrGenerateVertices(LineData line, bool Generate = false)
    {
        //if ((line.startTriangle + line.points.Length * 2) > mesh.vertexCount && !Generate) return;

        // Crear vértices con grosor
        for (int i = 0; i < line.points.Length; i++)
        {
            Vector2 dir = GetDirection(i, line);

            //esto es para darle grosor una vez tenemos la direccion a la cual mirara
            Vector2 normal = new Vector2(-dir.y, dir.x);

            //obtenemos la mitad del grosor de la linea para que la suma de el grosor
            float halfWidth = line.lineWidth * 0.5f;

            if (Generate)
            {
                vertices.Add(line.points[i] - (Vector3)(normal * halfWidth) + line.lineOrigin.position);

                vertices.Add(line.points[i] + (Vector3)(normal * halfWidth) + line.lineOrigin.position);

            }
            else
            {
                int t = i * 2;

                vertices[t + line.startVertex] = line.points[i] - (Vector3)(normal * halfWidth) + line.lineOrigin.position;
                vertices[t + 1 + line.startVertex] = line.points[i] + (Vector3)(normal * halfWidth) + line.lineOrigin.position;
            }
        }
    }

    private Vector2 GetDirection(int index, LineData line)
    {
        Vector2 dir;

        //verificamos si estamos en el primer punto
        if (index == 0)
        {
            dir = (line.points[index + 1] - line.points[index]).normalized;
        }
        //verificamos si estamos en el ultimo punto
        else if (index == line.points.Length - 1)
        {
            dir = (line.points[index] - line.points[index - 1]).normalized;
        }
        //si no entonces estamos entre medio de la linea
        else
        {
            //calcula la direccion a la que deberia mirar usando ambas direccion de referencia
            Vector2 prev = (line.points[index] - line.points[index - 1]).normalized;

            Vector2 next = (line.points[index + 1] - line.points[index]).normalized;

            dir = (prev + next).normalized;
        }

        return dir;
    }

    private void GenerateTriangles(LineData line, int startVertex)
    {
        // Crear triángulos
        for (int i = 0; i < line.points.Length - 1; i++)
        {
            int v = startVertex + i * 2;

            triangles.Add(v);
            triangles.Add(v + 1);
            triangles.Add(v + 2);

            triangles.Add(v + 1);
            triangles.Add(v + 3);
            triangles.Add(v + 2);
        }
    }

    private void OnDrawGizmos()
    {
        if (mesh == null || !debugDraw) return;

        for (int i = 0; i < triangles.Count; i += 3)
        {
            Debug.DrawLine(
                vertices[triangles[i]] + transform.position,
                vertices[triangles[i + 1]] + transform.position,
                Color.red);

            Debug.DrawLine(
                vertices[triangles[i + 1]] + transform.position,
                vertices[triangles[i + 2]] + transform.position,
                Color.red);

            Debug.DrawLine(
                vertices[triangles[i + 2]] + transform.position,
                vertices[triangles[i]] + transform.position,
                Color.red);
        }
    }
}
