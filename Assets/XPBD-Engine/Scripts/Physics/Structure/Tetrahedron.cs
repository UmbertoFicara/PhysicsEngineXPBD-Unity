using UnityEngine;

namespace XPBD_Engine.Scripts.Physics.Structure
{
    /// <summary>
    /// Represents a tetrahedron in 3D space.
    /// </summary>
    public struct Tetrahedron
    {

        private const float OneOverSix = 1f / 6f;
        /// <summary>
        /// <para>The first vertex of the tetrahedron.</para>
        /// </summary>
        public Vector3 v1;
        /// <summary>
        ///  <para>The second vertex of the tetrahedron.</para>
        /// </summary>
        public Vector3 v2;
        /// <summary>
        ///  <para>The third vertex of the tetrahedron.</para>
        /// </summary>
        public Vector3 v3;
        /// <summary>
        ///  <para>The fourth vertex of the tetrahedron.</para>
        /// </summary>
        public Vector3 v4;
        
        public Tetrahedron(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
            this.v4 = v4;
        }
        
        /// <summary>
        /// Calculates the volume of the tetrahedron instance.
        /// </summary>
        /// <returns>The volume of the tetrahedron.</returns>
        public float Volume()
        {
            float ret = Vector3.Dot(Vector3.Cross(v2 - v1, v3 - v1), v4 - v1) * OneOverSix;
            return ret;
        }
        
        /// <summary>
        /// Calculates the volume of a tetrahedron given its four vertices.
        /// </summary>
        /// <param name="v1">The first vertex of the tetrahedron.</param>
        /// <param name="v2">The second vertex of the tetrahedron.</param>
        /// <param name="v3">The third vertex of the tetrahedron.</param>
        /// <param name="v4">The fourth vertex of the tetrahedron.</param>
        /// <returns>The volume of the tetrahedron.</returns>
        public static float Volume(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            float ret = Vector3.Dot(Vector3.Cross(v2 - v1, v3 - v1), v4 - v1) * OneOverSix;
            return ret;
        }
        
    }
    
}