namespace NFMWorld.Util;

public static class Sys
{
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