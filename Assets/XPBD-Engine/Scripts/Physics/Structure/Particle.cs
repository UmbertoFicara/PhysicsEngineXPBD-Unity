using UnityEngine;

namespace XPBD_Engine.Scripts.Physics.Structure
{
    public struct Particle
    {
        public Vector3 Position;            //The position of the particle
        public Vector3 PrevPosition;        //The previous position of the particle
        public Vector3 Velocity;            //The velocity of the particle
        public float InvMass;               //The inverse mass of the particle 
    }
}