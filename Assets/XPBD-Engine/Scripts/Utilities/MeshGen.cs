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
    public int vertexResolution = 2;
    public float width;
    public float height;
    public float depth;
    public float gizmoSphereRadius;
    private TetMesh _tetMesh;
    private Mesh _mesh;
    public MeshTopology meshTopology; 

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

        _mesh.SetIndices(DrawIndices(totalVertices), meshTopology, 0);

        GetComponent<MeshFilter>().mesh = _mesh;
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

    private int[] DrawIndices(int totalVertices)
    {
        int[] indices;
        switch (meshTopology)
        {
            //Point Mesh
            case MeshTopology.Points:
            {
                indices = new int[totalVertices];

                for (int i = 0; i < totalVertices; i++)
                {
                    indices[i] = i;
                }
                
                break;
            }
            /* case MeshTopology.Triangles:
            {
                var cubes = MathF.Pow(vertexResolution - 1, 3);
                var lastIndex = 0;
                for(int i = 0; i < cubes; i++)
                {
                    //assuming vertices are arranged in cubes
                    var i000 = lastIndex;
                    var i001 = lastIndex + 1;
                    var i010 = lastIndex + 2;
                    var i011 = lastIndex + 3;
                    var i100 = lastIndex + 4;
                    var i101 = lastIndex + 5;
                    var i110 = lastIndex + 6;
                    var i111 = lastIndex + 7;

                    DrawTetMesh(i100, i101, i111, i011);
                    DrawTetMesh(i101, i110, i111, i011);
                    DrawTetMesh(i101, i001, i011, i010);
                    DrawTetMesh(i101, i010, i111, i010);
                    DrawTetMesh(i011, i010, i110, i111);
                }

                

                
                
                break;
            } */

            default: return null;
        }

        return indices;
    }

    private void DrawTetMesh()
    {

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
