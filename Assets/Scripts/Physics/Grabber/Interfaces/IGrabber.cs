using System.Collections.Generic;
using Physics.Grabber.Interfaces;
using UnityEngine;

namespace Grabber
{
    public interface IGrabber
    {
        public void StartGrab(List<IGrabbable> bodies);

        public void MoveGrab(Vector3 position);

        public void EndGrab(Vector3 position);

    }
}