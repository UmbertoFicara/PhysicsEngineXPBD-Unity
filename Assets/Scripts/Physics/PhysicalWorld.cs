using System;
using Physics.RigidBody;
using UnityEngine;

namespace Physics
{
    public class PhysicalWorld : MonoBehaviour
    {
        public Vector3 gravity;
        public Vector3 worldSize;
        public Vector3 worldCenter;
        public int numSubsteps = 10;
        public bool paused;

        private SoftBody.SoftBody[] _softBodies;
        private Ball[] _balls;

        private bool _isPressedJump;
        private bool _isPressedSqueeze;
        private void Start()
        {
            _softBodies = FindObjectsOfType<SoftBody.SoftBody>();
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

            HandleSimulationSoftBodys();
            HandleSimulationBalls();
        }
        private void HandleSimulationSoftBodys()
        {
            var sdt = Time.fixedDeltaTime / numSubsteps;
            for (var step = 0; step < numSubsteps; step++) {
                for (var i = 0; i < _softBodies.Length; i++) 
                    _softBodies[i].PreSolve(sdt, gravity,worldSize,worldCenter);
					
                for (var i = 0; i < _softBodies.Length; i++) 
                    _softBodies[i].Solve(sdt);

                for (var i = 0; i < _softBodies.Length; i++) 
                    _softBodies[i].PostSolve(sdt);
            }
        }
        private void HandleSimulationBalls()
        {
            for (var i = 0; i < _balls.Length; i++)
                _balls[i].Simulate(gravity,Time.fixedDeltaTime,worldSize);
        }
        public SoftBody.SoftBody[] GetSoftBodies()
        {
            return _softBodies;
        }
    }
}
