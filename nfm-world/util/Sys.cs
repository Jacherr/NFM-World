namespace NFMWorld.Util;

public static class Sys
{
    public static void ArrayCopy(int[] src, int srcPos, int[] dest, int destPos, int length)
    {
        Array.Copy(src, srcPos, dest, destPos, length);
    }

    public static void ArrayCopy<T>(T[] src, int srcPos, T[] dest, int destPos, int length)
    {
        Array.Copy(src, srcPos, dest, destPos, length);
    }

    public static void RequestSleep(long ms)
    {
        // empty
    }

    public static void Exit(int i)
    {
        Environment.Exit(1);
    }

    public static float Cap(this float f)
    {
        return float.IsNaN(f) ? 0 : f;
    }

    public static double Cap(this double f)
    {
        return double.IsNaN(f) ? 0 : f;
    }

    public static float CapF(this double f)
    {
        return (float) (double.IsNaN(f) ? 0 : f);
    }
}