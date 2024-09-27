using UnityEngine;
using XPBD_Engine.Scripts.Physics.Structure;
using XPBD_Engine.Scripts.Utilities;

namespace XPBD_Engine.Scripts.Physics.SoftBody
{
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class SoftBody : MonoBehaviour
	{
		#region Variables
		public TextAsset modelJson;								
		[Range(0f,500f)]public float edgeCompliance= 100.0f;	
		[Range(0f,500f)]public float volCompliance = 0.0f;		
		
		public TetVisMesh tetVisMesh;
		private Mesh _mesh;								//Unity mesh	
		private Particle[] _particles;					//Particles
		private SoftBodyTetrahedron[] _tetrahedrons;	//Tetrahedrons
		private SoftBodyEdge[] _edges;					//Edges
		#endregion
		public Rigidbody2D rb;
		#region MonoBehaviour
		private void Awake()
		{
			StartSoftBody();
			InitMesh();
		}
		private void OnDestroy()
		{
			Destroy(_mesh);
		}
		

		/// <summary>
		/// Initializes the soft body by parsing the tetrahedral mesh data from the provided JSON file.
		/// </summary>
		private void StartSoftBody()
		{
			tetVisMesh = JsonUtility.FromJson<TetVisMesh>(modelJson.text);
			
			
			_particles = new Particle[tetVisMesh.verts.Length / 3];	
			_tetrahedrons = new SoftBodyTetrahedron[tetVisMesh.tetIds.Length / 4];
			_edges = new SoftBodyEdge[tetVisMesh.tetEdgeIds.Length / 2];
			
			for ( var i = 0; i < _particles.Length; i++)
			{
				var a = i * 3;
				_particles[i] = new Particle
				{
					Position = new Vector3(tetVisMesh.verts[a], tetVisMesh.verts[a + 1], tetVisMesh.verts[a + 2]),
					PrevPosition = new Vector3(tetVisMesh.verts[a], tetVisMesh.verts[a + 1], tetVisMesh.verts[a + 2]),
					Velocity = Vector3.zero,
					InvMass = 0f
				};
			}

			for (var i = 0; i < _tetrahedrons.Length; i++)
			{
				var a = i * 4;
				_tetrahedrons[i] = new SoftBodyTetrahedron
				{
					v1 = tetVisMesh.tetIds[a],
					v2 = tetVisMesh.tetIds[a + 1],
					v3 = tetVisMesh.tetIds[a + 2],
					v4 = tetVisMesh.tetIds[a + 3],
					restVolume = Tetrahedron.Volume(_particles[tetVisMesh.tetIds[a]].Position, _particles[tetVisMesh.tetIds[a + 1]].Position, _particles[tetVisMesh.tetIds[a + 2]].Position, _particles[tetVisMesh.tetIds[a + 3]].Position)
				};
			}
			
			for ( var i = 0; i < _edges.Length; i++)
			{
				var a = i * 2;
				_edges[i] = new SoftBodyEdge
				{
					v1 = tetVisMesh.tetEdgeIds[a],
					v2 = tetVisMesh.tetEdgeIds[a + 1],
					restLength = Vector3.Distance(_particles[tetVisMesh.tetEdgeIds[a]].Position, _particles[tetVisMesh.tetEdgeIds[a + 1]].Position)
				};
			}
			
			for (var i = 0; i < _tetrahedrons.Length; i++) {
				ref var tet = ref _tetrahedrons[i];
				var pInvMass = tet.restVolume > 0.0f ? 1.0f / (tet.restVolume / 4.0f) : 0.0f;
				_particles[tet.v1].InvMass += pInvMass;
				_particles[tet.v2].InvMass += pInvMass;
				_particles[tet.v3].InvMass  += pInvMass;
				_particles[tet.v4].InvMass  += pInvMass;
			}
		}

		
		
		private void OnDrawGizmos()
		{
			if (Application.isPlaying) return;
			if(modelJson==null) return;
			var tetVisMesh = JsonUtility.FromJson<TetVisMesh>(modelJson.text);
			var numParticles = tetVisMesh.verts.Length / 3;
			
			_particles = new Particle[numParticles];	
			
			var listPos = GetPosFromTetMesh(tetVisMesh.verts);

			for (var index = 0; index < _particles.Length; index++)
			{
				var pos = listPos[index];
				_particles[index] = new Particle
				{
					Position = pos,
					PrevPosition = pos,
					Velocity = Vector3.zero,
					InvMass = 1f / 1f
				};
			}
			// Draw a yellow sphere at the transform's position
			Gizmos.DrawMesh(GetMesh(tetVisMesh),transform.position);
		}

		private Vector3[] GetPosFromTetMesh(float[] vertices)
		{
			var numParticles = vertices.Length / 3;
			var pos = new Vector3[numParticles];
			
			var position = transform.position;
			var scale = transform.localScale;
			var rotation = Quaternion.Euler(transform.rotation.eulerAngles);
			
			for (var i = 0; i < numParticles; i++)
			{
				var a = i * 3;
				var vect = new Vector3(vertices[a], vertices[a + 1], vertices[a + 2]);
				vect = Vector3.Scale(vect, scale);
				vect = rotation * vect;
				pos[i] = vect + position;
			}
			return pos; 
		}
		#endregion
		
		#region Mesh
		//
		// Unity mesh 
		//
		//Init the mesh when the simulation is started
		private void InitMesh()
		{
			_mesh = GetMesh(tetVisMesh);
			Debug.Log(_mesh.vertexCount);
			GetComponent<MeshFilter>().mesh = _mesh;
		}

		private Mesh GetMesh(TetVisMesh tetMesh)
		{
			var mesh = new Mesh();
			mesh.Clear();

			var vertices = new Vector3[tetMesh.vertexUvList.Length/2];
			var uvs = new Vector2[tetMesh.vertexUvList.Length/2];
			var triangles = tetMesh.tetSurfaceVertexUvIds;

			var meshUvs = new Vector2[tetMesh.uvs.Length / 2];
			for (int i = 0; i < tetMesh.uvs.Length/2; i++)
			{
				meshUvs[i] = new Vector2(tetMesh.uvs[2 * i], tetMesh.uvs[2 * i + 1]);
			}
			for (int i = 0; i < tetMesh.vertexUvList.Length/2; i++)
			{
				vertices[i] = _particles[tetMesh.vertexUvList[2 * i + 0]].Position;
				uvs[i] = meshUvs[tetMesh.vertexUvList[2 * i + 1]];
			}
			mesh.vertices = vertices;
			mesh.uv = uvs;
			mesh.triangles = triangles;
			
			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
			return mesh;
		}
		private void UpdateMeshes() 
		{
			var vertices = new Vector3[tetVisMesh.vertexUvList.Length/2];
			for (int i = 0; i < tetVisMesh.vertexUvList.Length/2; i++)
			{
				vertices[i] = _particles[tetVisMesh.vertexUvList[2 * i + 0]].Position;
			}
			_mesh.vertices = vertices;
			_mesh.RecalculateBounds();
			_mesh.RecalculateNormals();
		}
		#endregion
		
		#region Physics
		public void PreSolve(float dt, Vector3 gravity,Vector3 worldBoundCenter,Vector3 worldBoundSize)
		{
			for (var i = 0; i < _particles.Length; i++) {
				var particle = _particles[i];
				particle.Velocity += gravity * dt;
				particle.PrevPosition = particle.Position;
				particle.Position += particle.Velocity * dt;
				_particles[i] = particle;
				
				EnvironmentCollision(i,worldBoundSize,worldBoundCenter);
			}
		}
		private void EnvironmentCollision(int i,Vector3 worldBoundSize,Vector3 worldBoundCenter)
		{
			
			var worldBoxMin = worldBoundCenter - worldBoundSize / 2.0f;
			var worldBoxMax = worldBoundCenter + worldBoundSize / 2.0f;
			ref var particle = ref _particles[i];
			// Floor collision
			var pos = particle.Position;

			// X
			if (pos.x < worldBoxMin.x || pos.x > worldBoxMax.x)
			{
				particle.Position = particle.PrevPosition;
				particle.Position.x = Mathf.Clamp(pos.x, worldBoxMin.x, worldBoxMax.x);
			}

			// Y
			if (pos.y < worldBoxMin.y || pos.y > worldBoxMax.y)
			{
				particle.Position = particle.PrevPosition;
				particle.Position.y = Mathf.Clamp(pos.y, worldBoxMin.y, worldBoxMax.y);
			}

			// Z
			if (pos.z < worldBoxMin.z || pos.z > worldBoxMax.z)
			{
				particle.Position = particle.PrevPosition;
				particle.Position.z = Mathf.Clamp(pos.z, worldBoxMin.z, worldBoxMax.z);
			}
		}
		public void Solve(float dt)
		{
			SolveEdges(edgeCompliance, dt);
			SolveVolumes(volCompliance, dt);
		}
		public void PostSolve(float dt)
		{
			var oneOverDt = 1f / dt;
			//For each particle
			for (var i = 0; i < _particles.Length; i++) {
				ref var particle = ref _particles[i];
				particle.Velocity = (particle.Position - particle.PrevPosition) * oneOverDt;
			}
			UpdateMeshes();
		}
		private void SolveEdges(float compliance,float dt)
		{
			var stiffness = 1.0f / compliance;
			var lambda = 0.0f;
			
			for (var i = 0; i < _edges.Length; i++) {
				ref var edge = ref _edges[i];
				var id0 = edge.v1;
				var id1 = edge.v2;
				ref var particle0 = ref _particles[id0];
				ref var particle1 = ref _particles[id1];
				
				XPBD.SolveDistanceConstraint(
					particle0.Position, particle0.InvMass, 
					particle1.Position, particle1.InvMass, 
					edge.restLength,
					stiffness,
					dt,
					ref lambda,
					out var corr0, out var corr1);
				
				particle0.Position += corr0;
				particle1.Position += corr1;
			}
		}
		private void SolveVolumes(float compliance,float dt)
		{
			var stiffness = 1.0f / compliance;
			var lambda = 0.0f;
			
			//For each tetra
			for (var i = 0; i < _tetrahedrons.Length; i++) {
				ref var tet = ref _tetrahedrons[i];
				//The 3 opposite vertices ids
				var id0  = tet.v1;
				var id1  = tet.v2;
				var id2  = tet.v3;
				var id3  = tet.v4;
				
				ref var particle0 = ref _particles[id0];
				ref var particle1 = ref _particles[id1];
				ref var particle2 = ref _particles[id2];
				ref var particle3 = ref _particles[id3];
				
				XPBD.SolveVolumeConstraint(
					particle0.Position, particle0.InvMass,
					particle1.Position, particle1.InvMass,
					particle2.Position, particle2.InvMass,
					particle3.Position, particle3.InvMass,
					tet.restVolume,
					 stiffness,
					dt,
					ref lambda,
					out var corr0, out var corr1, out var corr2, out var corr3);
				
				particle0.Position += corr0;
				particle1.Position += corr1;
				particle2.Position += corr2;
				particle3.Position += corr3;
			}
	    
		}
		#endregion
		
	}
}





