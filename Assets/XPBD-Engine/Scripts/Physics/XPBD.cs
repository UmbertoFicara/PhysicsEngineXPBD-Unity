using UnityEngine;

namespace XPBD_Engine.Scripts.Physics
{
    public static class XPBD
    {
        // Constants
        private const float eps = 1e-6f;

        /// <summary>
        /// Determine the position corrections for a distance constraint between two particles using XPBD:
        /// 
        /// C(p0, p1) = ||p0 - p1|| - l0 = 0
        /// 
        /// More information can be found in the following papers: Mueller07, BMOT2013, BMOTM2014, BMM2015, MMC16
        /// </summary>
        /// <param name="p0">Position of first particle</param>
        /// <param name="invMass0">Inverse mass of first particle</param>
        /// <param name="p1">Position of second particle</param>
        /// <param name="invMass1">Inverse mass of second particle</param>
        /// <param name="restLength">Rest length of distance constraint</param>
        /// <param name="stiffness">Stiffness coefficient</param>
        /// <param name="dt">Time step size</param>
        /// <param name="lambda">Lagrange multiplier (XPBD)</param>
        /// <param name="corr0">Position correction of first particle</param>
        /// <param name="corr1">Position correction of second particle</param>
        /// <returns>True if the constraint was successfully solved, false otherwise</returns>
        public static bool SolveDistanceConstraint(
            Vector3 p0, float invMass0,
            Vector3 p1, float invMass1,
            float restLength,
            float stiffness,
            float dt,
            ref float lambda,
            out Vector3 corr0, out Vector3 corr1)
        {
            float K = invMass0 + invMass1;
            Vector3 n = p0 - p1;
            float d = n.magnitude;
            float C = d - restLength;
            if (d > 1e-6f)
                n /= d;
            else
            {
                corr0 = Vector3.zero;
                corr1 = Vector3.zero;
                return true;
            }

            float alpha = 0.0f;
            if (stiffness != 0.0f)
            {
                alpha = 1.0f / (stiffness * dt * dt);
                K += alpha;
            }

            float Kinv = 0.0f;
            if (Mathf.Abs(K) > 1e-6f)
                Kinv = 1.0f / K;
            else
            {
                corr0 = Vector3.zero;
                corr1 = Vector3.zero;
                return true;
            }

            float delta_lambda = -Kinv * (C + alpha * lambda);
            lambda += delta_lambda;
            Vector3 pt = n * delta_lambda;
            corr0 = invMass0 * pt;
            corr1 = -invMass1 * pt;
            return true;
        }

        /// <summary>
        /// Determine the position corrections for a constraint that conserves the volume
        /// of a single tetrahedron. Such a constraint has the form:
        /// 
        /// C(p1, p2, p3, p4) = (1/6) * ((p2 - p1) × (p3 - p1)) · (p4 - p1) - V0,
        /// 
        /// where p1, p2, p3 and p4 are the four corners of the tetrahedron and V0 is its rest volume.
        /// 
        /// More information can be found in the following papers: Mueller07, BMOT2013, BMOTM2014, BMM2015
        /// </summary>
        /// <param name="p0">Position of first particle</param>
        /// <param name="invMass0">Inverse mass of first particle</param>
        /// <param name="p1">Position of second particle</param>
        /// <param name="invMass1">Inverse mass of second particle</param>
        /// <param name="p2">Position of third particle</param>
        /// <param name="invMass2">Inverse mass of third particle</param>
        /// <param name="p3">Position of fourth particle</param>
        /// <param name="invMass3">Inverse mass of fourth particle</param>
        /// <param name="restVolume">Rest volume V0</param>
        /// <param name="stiffness">Stiffness coefficient</param>
        /// <param name="dt">Time step size</param>
        /// <param name="lambda">Lagrange multiplier (XPBD)</param>
        /// <param name="corr0">Position correction of first particle</param>
        /// <param name="corr1">Position correction of second particle</param>
        /// <param name="corr2">Position correction of third particle</param>
        /// <param name="corr3">Position correction of fourth particle</param>
        /// <returns>True if the constraint was successfully solved, false otherwise</returns>
        public static bool SolveVolumeConstraint(
            Vector3 p0, float invMass0,
            Vector3 p1, float invMass1,
            Vector3 p2, float invMass2,
            Vector3 p3, float invMass3,
            float restVolume,
            float stiffness,
            float dt,
            ref float lambda,
            out Vector3 corr0, out Vector3 corr1, out Vector3 corr2, out Vector3 corr3)
        {
            float volume = (1.0f / 6.0f) * Vector3.Dot(Vector3.Cross(p1 - p0, p2 - p0), p3 - p0);
            corr0 = corr1 = corr2 = corr3 = Vector3.zero;

            Vector3 grad0 = Vector3.Cross(p1 - p2, p3 - p2);
            Vector3 grad1 = Vector3.Cross(p2 - p0, p3 - p0);
            Vector3 grad2 = Vector3.Cross(p0 - p1, p3 - p1);
            Vector3 grad3 = Vector3.Cross(p1 - p0, p2 - p0);

            float K =
                invMass0 * grad0.sqrMagnitude +
                invMass1 * grad1.sqrMagnitude +
                invMass2 * grad2.sqrMagnitude +
                invMass3 * grad3.sqrMagnitude;

            float alpha = 0.0f;
            if (stiffness != 0.0f)
            {
                alpha = 1.0f / (stiffness * dt * dt);
                K += alpha;
            }

            if (Mathf.Abs(K) < eps)
            {
                corr0 = corr1 = corr2 = corr3 = Vector3.zero;
                return false;
            }

            float C = volume - restVolume;
            float delta_lambda = -(C + alpha * lambda) / K;
            lambda += delta_lambda;

            corr0 = delta_lambda * invMass0 * grad0;
            corr1 = delta_lambda * invMass1 * grad1;
            corr2 = delta_lambda * invMass2 * grad2;
            corr3 = delta_lambda * invMass3 * grad3;

            return true;
        }
    }
}
