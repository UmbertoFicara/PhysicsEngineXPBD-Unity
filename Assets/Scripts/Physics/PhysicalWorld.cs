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
        private bool _isSqueezed;
        private void Start()
        {
            _softBodies = FindObjectsOfType<SoftBody.SoftBody>();
            _balls = FindObjectsOfType<Ball>();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                _isPressedJump = true;
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                paused = !paused;
                _isSqueezed = false;
            }
            //Make the mesh flat when holding right mouse 
            if (Input.GetKeyDown(KeyCode.L))
            {
                if (_isSqueezed)
                {
                    _isSqueezed = false;
                    paused = false;
                }
                else
                {
                    _isPressedSqueeze = true;
                    paused = true;
                }
     
            }
            
        }
        private void FixedUpdate()
        {
            if (_isPressedJump)
            {
                _isPressedJump = false;
                foreach (var softBody in _softBodies)
                {
                    softBody.Translate(new Vector3(0,5,0));
                }
            }

            if (_isPressedSqueeze)
            {
                _isPressedSqueeze = false;
                foreach (var soft in _softBodies)
                {
                    soft.Squeeze();
                }

                _isSqueezed = true;
            }
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
