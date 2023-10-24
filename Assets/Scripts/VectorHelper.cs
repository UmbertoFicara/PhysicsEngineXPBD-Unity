namespace DefaultNamespace
{
    public static class VectorHelper
    {
        public static void VecSetZero(float[] a, int anr)
        {
            anr *= 3;
            a[anr++] = 0.0f;
            a[anr++] = 0.0f;
            a[anr] = 0.0f;
        }

        public static void VecScale(float[] a, int anr, float scale)
        {
            anr *= 3;
            a[anr++] *= scale;
            a[anr++] *= scale;
            a[anr] *= scale;
        }

        public static void VecCopy(float[] a, int anr, float[] b, int bnr)
        {
            anr *= 3;
            bnr *= 3;
            a[anr++] = b[bnr++];
            a[anr++] = b[bnr++];
            a[anr] = b[bnr];
        }

        public static void VecAdd(float[] a, int anr, float[] b, int bnr, float scale = 1.0f)
        {
            anr *= 3;
            bnr *= 3;
            a[anr++] += b[bnr++] * scale;
            a[anr++] += b[bnr++] * scale;
            a[anr] += b[bnr] * scale;
        }
        public static void VecSetDiff(float[] dst, int dnr, float[] a, int anr, float[] b, int bnr, float scale = 1.0f)
        {
            dnr *= 3; anr *= 3; bnr *= 3;
            dst[dnr++] = (a[anr++] - b[bnr++]) * scale;
            dst[dnr++] = (a[anr++] - b[bnr++]) * scale;
            dst[dnr] = (a[anr] - b[bnr]) * scale;
        }

        public static float VecLengthSquared(float[] a, int anr)
        {
            anr *= 3;
            float a0 = a[anr];
            float a1 = a[anr + 1];
            float a2 = a[anr + 2];
            return a0 * a0 + a1 * a1 + a2 * a2;
        }

        public static float VecDistSquared(float[] a, int anr, float[] b, int bnr)
        {
            anr *= 3; bnr *= 3;
            float a0 = a[anr] - b[bnr];
            float a1 = a[anr + 1] - b[bnr + 1];
            float a2 = a[anr + 2] - b[bnr + 2];
            return a0 * a0 + a1 * a1 + a2 * a2;
        }

        public static float VecDot(float[] a, int anr, float[] b, int bnr)
        {
            anr *= 3; bnr *= 3;
            return a[anr] * b[bnr] + a[anr + 1] * b[bnr + 1] + a[anr + 2] * b[bnr + 2];
        }

        public static void VecSetCross(float[] a, int anr, float[] b, int bnr, float[] c, int cnr)
        {
            anr *= 3; bnr *= 3; cnr *= 3;
            a[anr++] = b[bnr + 1] * c[cnr + 2] - b[bnr + 2] * c[cnr + 1];
            a[anr++] = b[bnr + 2] * c[cnr + 0] - b[bnr + 0] * c[cnr + 2];
            a[anr] = b[bnr + 0] * c[cnr + 1] - b[bnr + 1] * c[cnr + 0];
        }

        
    }
}