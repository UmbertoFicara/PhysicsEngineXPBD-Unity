using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter))]
public class SoftBodyVec3 : MonoBehaviour
{
	public TextAsset modelJson;
	public Model tetMesh = new Model();

	private Mesh _mesh;
	
	[Range(0f,500f)]public float edgeCompliance= 100.0f;
	[Range(0f,500f)]public float volCompliance = 0.0f;

	private int _numParticles;
	private int _numTets;
	private Vector3[] _pos; //Use an array of float instead of a list of vect3 because is more efficient and the mesh in are usually stored in this way
	private Vector3[] _prevPos;
	private Vector3[] _vel;

	private int[] _tetIds;
	private int[] _edgeIds;
	private float[] _restVol;
	private float[] _edgeLengths;
	private float[] _invMass;

	private float _edgeCompliance;
	private float _volCompliance;
	
	private Vector3[] _temp;
	private Vector3[] _grads;

	private int[][] _volIdOrder;

	
	// Start is called before the first frame update
	void Start()
	{
		tetMesh = JsonUtility.FromJson<Model>(modelJson.text);
		
		_numParticles = tetMesh.verts.Length / 3;
		_numTets = tetMesh.tetIds.Length / 4;
		var temp = new Vector3[_numParticles];
		for (var i = 0; i < _numParticles; i++)
		{
			var a = i * 3;
			var tempVect = new Vector3(tetMesh.verts[a], tetMesh.verts[a + 1], tetMesh.verts[a + 2]);
			temp[i] = tempVect;
		}
		_pos = temp;
		_prevPos = temp;
		_vel = new Vector3[_numParticles];

		_tetIds = tetMesh.tetIds;
		_edgeIds = tetMesh.tetEdgeIds;
		_restVol = new float[_numTets];
		_edgeLengths = new float[_edgeIds.Length / 2];	
		_invMass = new float[_numParticles];
		
		_edgeCompliance = edgeCompliance;
		_volCompliance = volCompliance;

		_temp = new Vector3[4];
		_grads = new Vector3[4];
		
		Translate(0,1,0);

		InitPhysics();
		

		_mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = _mesh;
		_mesh.Clear();
		_mesh.vertices = _pos;
		_mesh.triangles = tetMesh.tetSurfaceTriIds;
		_mesh.RecalculateBounds();
		_mesh.RecalculateNormals();
		

		_volIdOrder = new[]
		{
			new[] { 1, 3, 2 },
			new[] { 0, 2, 3 },
			new[] { 0, 3, 1 },
			new[] { 0, 1, 2 }
		};
	}

    
    public void Translate(float x,float y,float z)
    {
	    for (var i = 0; i < _numParticles; i++)
	    {
		    _pos[i] += new Vector3(x, y, z);
		    _prevPos[i] += new Vector3(x, y, z);
	    }
    }
    private void UpdateMeshes() 
    {
	    _mesh.vertices = _pos;
	    _mesh.RecalculateBounds();
	    _mesh.RecalculateNormals();
    }
    private float GetTetVolume(int nr) 
    {
	    var id0 = _tetIds[4 * nr];
	    var id1 = _tetIds[4 * nr + 1];
	    var id2 = _tetIds[4 * nr + 2];
	    var id3 = _tetIds[4 * nr + 3];
	    _temp[0] = _pos[id1] - _pos[id0];
	    _temp[1] = _pos[id2] - _pos[id0];
	    _temp[2] = _pos[id3] - _pos[id0];
	    _temp[3]=Vector3.Cross(_temp[0], _temp[1]);
	    return Vector3.Dot(_temp[3], _temp[2]) / 6f;
    }
    
    private void InitPhysics()
    {
	    for (var i = 0; i < _numTets; i++) {
		    var vol =GetTetVolume(i);
		    _restVol[i] = vol;
		    var pInvMass = vol > 0.0f ? 1.0f / (vol / 4.0f) : 0.0f;
		    _invMass[_tetIds[4 * i]] += pInvMass;
		    _invMass[_tetIds[4 * i + 1]] += pInvMass;
		    _invMass[_tetIds[4 * i + 2]] += pInvMass;
		    _invMass[_tetIds[4 * i + 3]] += pInvMass;
	    }
	    for (var i = 0; i < _edgeLengths.Length; i++) {
		    var id0 = _edgeIds[2 * i];
		    var id1 = _edgeIds[2 * i + 1];
		    _edgeLengths[i] = Vector3.Distance(_pos[id0], _pos[id1]);
	    }
    }
    
    public void PreSolve(float dt, Vector3 gravity)
    {
	    for (var i = 0; i < _numParticles; i++) {
		    if (_invMass[i] == 0.0)
			    continue;
		    _vel[i] += gravity * dt;
		    _prevPos[i] = _pos[i];
		    _pos[i] += _vel[i] * dt;
		    var y = _pos[i].y;
		    if (y < 0.0) {
			    _pos[i]=_prevPos[i];
			    _pos[i].y = 0.0f;
		    }
	    }
	    
    }
    public void Solve(float dt)
    {
	    SolveEdges(edgeCompliance, dt);
	    SolveVolumes(volCompliance, dt);
    }
    public void PostSolve(float dt)
    {
	    for (var i = 0; i < _numParticles; i++) {
		    if (_invMass[i] == 0.0)
			    continue;
		    _vel[i] = (_pos[i] - _prevPos[i]) / dt;
	    }
	    UpdateMeshes();
    }
    
    private void SolveEdges(float compliance,float dt)
    {
	    var alpha = compliance / dt /dt;

	    for (var i = 0; i < _edgeLengths.Length; i++) {
		    var id0 = _edgeIds[2 * i];
		    var id1 = _edgeIds[2 * i + 1];
		    var w0 = _invMass[id0];
		    var w1 = _invMass[id1];
		    var w = w0 + w1;
		    if (w == 0.0)
			    continue;

		    _grads[0] = _pos[id0] - _pos[id1];
		    var len = Vector3.Magnitude(_grads[0]);
		    if (len == 0.0)
			    continue;
		    _grads[0] *= 1.0f / len;
		    var restLen = _edgeLengths[i];
		    var C = len - restLen;
		    var s = -C / (w + alpha);
		    _pos[id0] += _grads[0] * (s * w0);
		    _pos[id1] += _grads[0] * (-s * w1);

	    }
    }
    private void SolveVolumes(float compliance,float dt)
    {
	    var alpha = compliance / dt /dt;

	    for (var i = 0; i < _numTets; i++) {
		    float w = 0.0f;
						
		    for (var j = 0; j < 4; j++) {
			    var id0 = _tetIds[4 * i + _volIdOrder[j][0]];
			    var id1 = _tetIds[4 * i + _volIdOrder[j][1]];
			    var id2 = _tetIds[4 * i + _volIdOrder[j][2]];

			    _temp[0] = _pos[id1] - _pos[id0];
			    _temp[1] = _pos[id2] - _pos[id0];
			    _grads[j] = Vector3.Cross(_temp[0], _temp[1]);
			    _grads[j] *= 1.0f / 6.0f;

			    w += _invMass[_tetIds[4 * i + j]] * Vector3.SqrMagnitude(_grads[j]);
		    }
		    if (w == 0.0)
			    continue;

		    var vol = GetTetVolume(i);
		    var restVol = _restVol[i];
		    var C = vol - restVol;
		    var s = -C / (w + alpha);

		    for (var j = 0; j < 4; j++) {
			    var id = _tetIds[4 * i + j];
			    _pos[id] += _grads[j] * (s * _invMass[id]);
		    }
	    }
	    
    }

    
}



