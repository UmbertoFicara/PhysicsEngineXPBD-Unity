using UnityEngine;
using Utilities.Data_structures;

namespace Physics.Grabber.Interfaces
{
    public interface IGrabbable
    {
        public int StartGrab(Vector3 grabPos);

        public void StartGrabVertex(Vector3 grabPos, int vertexIndex);

        public void MoveGrabbed(Vector3 grabPos,int vertexIndex);

        public void EndGrab(Vector3 grabPos, Vector3 vel,int vertexIndex);

        public void IsRayHittingBody(Ray ray, out PointerHit hit);

        public bool IsSphereInsideBody(Vector3 center,float radius, out SphereHit bestVertex);

        public Vector3 GetGrabbedPos(int vertexIndex);
    }
}
