using Physics.Grabber.Interfaces;

namespace Utilities.Data_structures
{
    //This class helps to record which body and vertex of that body it is grabbing
    public struct GrabbedBodyVertex
    {
        public readonly IGrabbable grabbedBody;
        public readonly int indexVertex;

        public GrabbedBodyVertex(IGrabbable grabbedBody, int indexVertex)
        {
            this.grabbedBody = grabbedBody;
            this.indexVertex = indexVertex;
        }
    }
}