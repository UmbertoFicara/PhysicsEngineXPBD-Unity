using UnityEngine;
using Utilities.Data_structures;

namespace Grabber
{
    public interface IGrabbable
    {
        public void StartGrab(Vector3 grabPos);

        public void MoveGrabbed(Vector3 grabPos);

        public void EndGrab(Vector3 grabPos, Vector3 vel);

        public void IsRayHittingBody(Ray ray, out CustomHit hit);

        public void IsSphereInsideBody(Vector3 center,float radius, out SphereHit hit);

        public Vector3 GetGrabbedPos();
    }
}
