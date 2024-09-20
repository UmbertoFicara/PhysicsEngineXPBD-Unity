using UnityEngine;
using XPBD_Engine.Scripts.Utilities;

[ExecuteInEditMode]
public class MeshGen : MonoBehaviour
{
    public int vertexResolution = 10;
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
        };

        _mesh = new Mesh
        {
            vertices = ConvertToVector3List(_tetMesh.verts)
        };
        _mesh.SetIndices(DrawIndices(totalVertices, 0), MeshTopology.Points, 0);
    }

    private float[] DrawVertices(int totalVertices)
    {
        
        float[] vertices = new float[3 * totalVertices];

        for (int k = 0; k < vertexResolution; k++)
        {
            for (int j = 0; j < vertexResolution; j++)
            {
                for (int i = 0; i < vertexResolution; i++)
                {
                    var x = i * width / vertexResolution;
                    var y = j * height / vertexResolution;
                    var z = k * depth / vertexResolution;
                    vertices[3 * (i + vertexResolution * j + vertexResolution * vertexResolution * k)] = x;
                    vertices[3 * (i + vertexResolution * j + vertexResolution * vertexResolution * k) + 1] = y;
                    vertices[3 * (i + vertexResolution * j + vertexResolution * vertexResolution * k) + 2] = z;

                    var vertexId = i + j + k;
                    //Debug.Log("vertex " + vertexId + ": " + x + ", " + y + ", " + z);
                }
            }
        }

        return vertices;
    }

    private int[] DrawIndices(int totalVertices, int meshTopology)
    {
        int[] indices;
        switch (meshTopology)
        {
            //Point Mesh
            case 0:
            {
                indices = new int[totalVertices];

                for (int i = 0; i < totalVertices; i++)
                {
                    indices[i] = i;
                }
                
                break;
            }
            default: return null;
        }

        return indices;
    }

    private void OnDrawGizmos()
    {
        if (_tetMesh == null) return;
        for (int k = 0; k < vertexResolution; k++)
        {
            for (int j = 0; j < vertexResolution; j++)
            {
                for (int i = 0; i < vertexResolution; i++)
                {
                    var x = _tetMesh.verts[3 * (i + vertexResolution * j + vertexResolution * vertexResolution * k)];
                    var y = _tetMesh.verts[3 * (i + vertexResolution * j + vertexResolution * vertexResolution * k) + 1];
                    var z = _tetMesh.verts[3 * (i + vertexResolution * j + vertexResolution * vertexResolution * k) + 2];
                    
                    Gizmos.DrawSphere(new Vector3(x,y,z), gizmoSphereRadius);
                }
            }
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
