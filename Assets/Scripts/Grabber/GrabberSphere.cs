using System.Collections.Generic;
using UnityEngine;
using Utilities.Data_structures;

namespace Grabber
{
    public class GrabberSphere : IGrabber
    {
        private Vector3 _centerPos;
        private float _radius;
        //The mesh we grab
        private IGrabbable _grabbedBody;
        //To give the mesh a velocity when we release it
        private Vector3 _lastGrabPos;
        
        public GrabberSphere(Vector3 center,float radius )
        {
            _centerPos = center;
            _radius = radius;
        }

        public void StartGrab(List<IGrabbable> bodies)
        {
            if (_grabbedBody != null)
            {
                return;
            }
            float maxDist = float.MaxValue;
            IGrabbable closestBody = null;
            foreach (var body in bodies)
            {
                body.IsSphereInsideBody(_centerPos, _radius, out SphereHit hit);
                if (hit!=null)
                {
                    if (hit.distance < maxDist)
                    {
                        closestBody = body;

                        maxDist = hit.distance;
                    }
                }
            }
            if (closestBody != null)
            {
                _grabbedBody = closestBody;
                //StartGrab is finding the closest vertex and setting it to the position where the ray hit the triangle
                closestBody.StartGrab(_centerPos);
                _lastGrabPos = _centerPos;
            }

        }

        public void MoveGrab(Vector3 position)
        {
            _centerPos = position;
            if (_grabbedBody == null)
            {
                return;
            }
            //Cache the old pos before we assign it
            _lastGrabPos = _grabbedBody.GetGrabbedPos();

            //Moved the vertex to the new pos
            _grabbedBody.MoveGrabbed(_centerPos);
        }

        public void EndGrab(Vector3 position)
        {
            if (_grabbedBody == null)
            {
                return;
            }

            //Add a velocity to the ball
            Vector3 grabPos = position;

            float vel = (grabPos - _lastGrabPos).magnitude / Time.deltaTime;
        
            Vector3 dir = (grabPos - _lastGrabPos).normalized;

            _grabbedBody.EndGrab(grabPos, dir * vel);

            _grabbedBody = null;
        }
    }
}