using System;
using System.Threading;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XPBD_Engine.Scripts.Utilities;

//[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class MeshGen : MonoBehaviour
{
    public bool displayEdges = true;  // Toggle for edge display
    public bool useGameObjectTransforms;
    public int vertexResolution = 2;
    public float width;
    public float height;
    public float depth;
    public float gizmoSphereRadius;
    private TetMesh _tetMesh;
    private Mesh _mesh;

    // Start is called before the first frame update
    void Start()
    {
        InitMesh();
        PrintVerticesPositions();
    }
    
    private void OnDestroy()
    {
        Destroy(_mesh);
    }

    public void InitMesh()
    {
        int totalVertices = vertexResolution * vertexResolution * vertexResolution;
        
        _tetMesh = new TetMesh
        {
            verts = DrawVertices(totalVertices)
            //verts = DrawTetrahedron();
        };

        _mesh = new Mesh
        {
            vertices = ConvertToVector3List(_tetMesh.verts)
        };

        _mesh.SetIndices(DrawIndices(), MeshTopology.Triangles, 0);

        GetComponent<MeshFilter>().mesh = _mesh;
    }

    private float[] DrawVertices(int totalVertices)
    {
        
        float[] vertices = new float[3 * totalVertices];

        // Calculate spacing between vertices for each dimension
        float xSpacing = width / (vertexResolution - 1);
        float ySpacing = height / (vertexResolution - 1);
        float zSpacing = depth / (vertexResolution - 1);

        int index = 0;
        
        for (int k = 0; k < vertexResolution; k++)
        {
            for (int j = 0; j < vertexResolution; j++)
            {
                for (int i = 0; i < vertexResolution; i++)
                {
                    // Calculate the position of each vertex
                    float x = i * xSpacing;
                    float y = j * ySpacing;
                    float z = k * zSpacing;

                    // Store the vertex position in the array
                    vertices[index++] = x;
                    vertices[index++] = y;
                    vertices[index++] = z;
                }
            }
        }

        return vertices;
    }

    private int[] DrawIndices()
    {
        int quadsPerDimension = vertexResolution - 1; // Number of quads per dimension
        int trianglesPerQuad = 2; // Two triangles per quad
        int verticesPerTriangle = 3; // Three vertices per triangle
        
        int totalQuads = quadsPerDimension * quadsPerDimension * quadsPerDimension * 6; // 6 faces on a cube
        int[] indices = new int[totalQuads * trianglesPerQuad * verticesPerTriangle];
        
        int index = 0; // To track the position in the indices array
        
        for (int k = 0; k < quadsPerDimension; k++)
        {
            for (int j = 0; j < quadsPerDimension; j++)
            {
                for (int i = 0; i < quadsPerDimension; i++)
                {
                    // Calculate the starting index of the 8 vertices of the cube cell in the grid
                    int vertexIndex = i + j * vertexResolution + k * vertexResolution * vertexResolution;
                    
                    // 8 vertices of the cube cell
                    int v0 = vertexIndex;
                    int v1 = vertexIndex + 1;
                    int v2 = vertexIndex + vertexResolution;
                    int v3 = vertexIndex + vertexResolution + 1;
                    int v4 = vertexIndex + vertexResolution * vertexResolution;
                    int v5 = vertexIndex + vertexResolution * vertexResolution + 1;
                    int v6 = vertexIndex + vertexResolution * vertexResolution + vertexResolution;
                    int v7 = vertexIndex + vertexResolution * vertexResolution + vertexResolution + 1;

                    // Front face (v0, v1, v2, v3)
                    indices[index++] = v0; indices[index++] = v2; indices[index++] = v1;
                    indices[index++] = v1; indices[index++] = v2; indices[index++] = v3;

                    // Back face (v4, v5, v6, v7)
                    indices[index++] = v4; indices[index++] = v5; indices[index++] = v6;
                    indices[index++] = v5; indices[index++] = v7; indices[index++] = v6;

                    // Top face (v2, v3, v6, v7)
                    indices[index++] = v2; indices[index++] = v6; indices[index++] = v3;
                    indices[index++] = v3; indices[index++] = v6; indices[index++] = v7;

                    // Bottom face (v0, v1, v4, v5)
                    indices[index++] = v0; indices[index++] = v4; indices[index++] = v1;
                    indices[index++] = v1; indices[index++] = v4; indices[index++] = v5;

                    // Left face (v0, v2, v4, v6)
                    indices[index++] = v0; indices[index++] = v4; indices[index++] = v2;
                    indices[index++] = v2; indices[index++] = v4; indices[index++] = v6;

                    // Right face (v1, v3, v5, v7)
                    indices[index++] = v1; indices[index++] = v3; indices[index++] = v5;
                    indices[index++] = v3; indices[index++] = v7; indices[index++] = v5;
                }
            }
        }

        return indices;
    }

    public void PrintVerticesPositions()
    {
        Debug.ClearDeveloperConsole();
        
        // Ensure the mesh exists
        if (_mesh == null)
        {
            Debug.LogWarning("Mesh not generated yet.");
            return;
        }

        Vector3[] vertices = _mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            Debug.Log($"Vertex {i}: Position = {vertices[i]}");
        }
    }


    private void OnDrawGizmos()
    {
        if (_mesh == null || !displayEdges) return;

        Gizmos.color = Color.cyan;
        Vector3[] vertices = _mesh.vertices;
        int[] indices = _mesh.triangles;

        // Loop through each triangle and draw the edges
        for (int i = 0; i < indices.Length; i += 3)
        {
            Vector3 v0 = transform.TransformPoint(vertices[indices[i]]);
            Vector3 v1 = transform.TransformPoint(vertices[indices[i + 1]]);
            Vector3 v2 = transform.TransformPoint(vertices[indices[i + 2]]);

            Gizmos.DrawLine(v0, v1);
            Gizmos.DrawLine(v1, v2);
            Gizmos.DrawLine(v2, v0);
        }
    }

    private Vector3[] ConvertToVector3List(float[] floatArray)
    {
        int vectorCount = floatArray.Length / 3;
        Vector3[] vectorArray = new Vector3[vectorCount];
        
        for (int i = 0; i < vectorCount; i++)
        {
            vectorArray[i] = new Vector3(floatArray[i * 3], floatArray[i * 3 + 1], floatArray[i * 3 + 2]);
        }

        return vectorArray;
    }
}
