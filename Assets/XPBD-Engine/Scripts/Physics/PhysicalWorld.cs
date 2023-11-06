using System.Collections.Generic;
using UnityEngine;
using XPBD_Engine.Scripts.Physics.RigidBody;
using XPBD_Engine.Scripts.Physics.SoftBody;

namespace XPBD_Engine.Scripts.Physics
{
    public class PhysicalWorld : MonoBehaviour
    {
        public Vector3 gravity;
        public Vector3 worldSize;
        public Vector3 worldCenter;
        public int numSubsteps = 10;
        public bool paused;

        private global::XPBD_Engine.Scripts.Physics.SoftBody.SoftBodyClassic[] _classicSoftBodies;
        private global::XPBD_Engine.Scripts.Physics.SoftBody.SoftBodyAdvanced[] _advancedSoftBodies;
        private Ball[] _balls;

        private bool _isPressedJump;
        private bool _isPressedSqueeze;
        private void Start()
        {
            _classicSoftBodies = FindObjectsOfType<global::XPBD_Engine.Scripts.Physics.SoftBody.SoftBodyClassic>();
            _advancedSoftBodies = FindObjectsOfType<global::XPBD_Engine.Scripts.Physics.SoftBody.SoftBodyAdvanced>();
            _balls = FindObjectsOfType<Ball>();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                paused = !paused;
            }
        }
        private void FixedUpdate()
        {
            Simulate();
        }

    
        private void Simulate()
        {
            if (paused)
                return;

            var sdt = Time.fixedDeltaTime / numSubsteps;
            HandleSimulationSoftBodyClassic(sdt);
            HandleSimulationSoftBodyAdvanced(sdt);
            HandleSimulationBalls();
        }
        private void HandleSimulationSoftBodyClassic(float sdt)
        {
            if (_classicSoftBodies.Length ==0)
                return;
            for (var step = 0; step < numSubsteps; step++) {
                for (var i = 0; i < _classicSoftBodies.Length; i++) 
                    _classicSoftBodies[i].PreSolve(sdt, gravity,worldSize,worldCenter);
					
                for (var i = 0; i < _classicSoftBodies.Length; i++) 
                    _classicSoftBodies[i].Solve(sdt);

                for (var i = 0; i < _classicSoftBodies.Length; i++) 
                    _classicSoftBodies[i].PostSolve(sdt);
            }
        }
        private void HandleSimulationSoftBodyAdvanced(float sdt)
        {
            if (_advancedSoftBodies.Length ==0)
                return;
            for (var step = 0; step < numSubsteps; step++) {
                for (var i = 0; i < _advancedSoftBodies.Length; i++) 
                    _advancedSoftBodies[i].PreSolve(sdt, gravity,worldSize,worldCenter);
					
                for (var i = 0; i < _advancedSoftBodies.Length; i++) 
                    _advancedSoftBodies[i].Solve(sdt);

                for (var i = 0; i < _advancedSoftBodies.Length; i++) 
                    _advancedSoftBodies[i].PostSolve(sdt);
            }
            //for (var i = 0; i < _advancedSoftBodies.Length; i++) 
            //    _advancedSoftBodies[i].UpdateMeshes();
        }
        private void HandleSimulationBalls()
        {
            if (_balls.Length ==0)
                return;
            for (var i = 0; i < _balls.Length; i++)
                _balls[i].Simulate(gravity,Time.fixedDeltaTime,worldSize);
        }
        public IEnumerable<SoftBodyClassic> GetSoftBodies()
        {
            return _classicSoftBodies;
        }
    }
}
