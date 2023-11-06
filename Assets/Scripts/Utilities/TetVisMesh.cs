namespace Utilities
{
    [System.Serializable]
    public class TetVisMesh
    {
        public string name;
        public float[] verts;
        public int[] tetIds;
        public int[] tetEdgeIds;
        public int[] tetSurfaceTriIds;
    }
}