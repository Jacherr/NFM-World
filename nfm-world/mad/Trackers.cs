using NFMWorld.Util;

namespace NFMWorld.Mad;

internal class Trackers
{
    //private Trackers() {}
    internal static readonly int[][] C = ArrayExt.New<int>(75000, 3);

    internal static readonly UnlimitedArray<int> Dam = [];
    internal static readonly UnlimitedArray<bool> Decor = [];
    internal static int Ncx;
    internal static int Ncz;
    internal static readonly UnlimitedArray<bool> Notwall = [];
    internal static int Nt = 0;
    internal static readonly UnlimitedArray<int> Radx = [];
    internal static readonly UnlimitedArray<int> Rady = [];
    internal static readonly UnlimitedArray<int> Radz = [];
    internal static readonly UnlimitedArray<int> Skd = [];
    internal static int Sx;
    internal static int Sz;
    internal static readonly UnlimitedArray<int> X = [];
    internal static readonly UnlimitedArray<int> Xy = [];
    internal static readonly UnlimitedArray<int> Y = [];
    internal static readonly UnlimitedArray<int> Z = [];
    internal static readonly UnlimitedArray<int> Zy = [];

    internal static void Devidetrackers(int sx, int ncx, int sz, int ncz)
    {
        Sx = sx;
        Sz = sz;
        Ncx = ncx / 3000;
        if (Ncx <= 0)
        {
            Ncx = 1;
        }
        Ncz = ncz / 3000;
        if (Ncz <= 0)
        {
            Ncz = 1;
        }
        
        // maxine: remove trackers.sect which was assigned here. it's not used anymore.
        
        for (var i = 0; i < Nt; i++)
        {
            if (Dam[i] == 167)
            {
                Dam[i] = 1;
            }
        }

        Ncx--;
        Ncz--;
    }
}