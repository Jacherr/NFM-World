using System.Numerics;
using System.Numerics.Tensors;
using System.Runtime.CompilerServices;

namespace NFMWorld.Util;

public static class UMath
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SafeAbs(int value)
    {
        return value == int.MinValue ? int.MaxValue : Math.Abs(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void VectorAdd<T>(in ReadOnlySpan<T> a, T b, in Span<T> destination)
        where T : IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>
    {
        TensorPrimitives.Add(a, b, destination);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void VectorNegate<T>(in ReadOnlySpan<T> a, in Span<T> destination)
        where T : IUnaryNegationOperators<T, T>
    {
        TensorPrimitives.Negate(a, destination);
    }

    public static void VectorRotate<T>(
        Span<T> a, Span<T> b, T offA, T offB, float sin, float cos
    )
        where T : unmanaged, ISubtractionOperators<T, T, T>, IAdditiveIdentity<T, T>, INumber<T>
    {
        // TODO see if some of these allocations can be avoided
        
        // move (a, b) to (offA, offB)
        Span<T> oa = stackalloc T[a.Length];
        Span<T> ob = stackalloc T[b.Length];
        TensorPrimitives.Subtract(a, offA, oa); // a - offA
        TensorPrimitives.Subtract(b, offB, ob); // b - offB

        Span<T> oaSin = stackalloc T[a.Length];
        Span<T> oaCos = oa;
        Span<T> obSin = stackalloc T[b.Length];
        Span<T> obCos = ob;
        TensorPrimitives.Multiply(oa, T.CreateTruncating(sin), oaSin);
        TensorPrimitives.Multiply(oa, T.CreateTruncating(cos), oaCos);
        TensorPrimitives.Multiply(ob, T.CreateTruncating(sin), obSin);
        TensorPrimitives.Multiply(ob, T.CreateTruncating(cos), obCos);
        
        TensorPrimitives.Subtract(oaCos, obSin, oa);
        TensorPrimitives.Add(oa, offA, a);
        
        TensorPrimitives.Add(oaSin, obCos, ob);
        TensorPrimitives.Add(ob, offB, b);
        
        // Functionally equivalent to:
        // for (var i = 0; i < a.Length; i++)
        // {
        //     var pa = a[i];
        //     var pb = b[i];
        //     var oa = (pa - offA);
        //     var ob = (pb - offB);
        //     var oaCos = oa * cos;
        //     var obSin = ob * sin;
        //     a[i] = offA + (oaCos - obSin);
        //     var oaSin = oa * sin;
        //     var obCos = ob * cos;
        //     b[i] = offB + (oaSin + obCos);
        // }
    }

    public static void VectorRotateCast<T, TCast>(
        Span<TCast> a, Span<TCast> b, TCast _offA, TCast _offB, float sin, float cos
    )
        where T : unmanaged, ISubtractionOperators<T, T, T>, IAdditiveIdentity<T, T>, INumber<T>
        where TCast : unmanaged, INumber<TCast>
    {
        var offA = T.CreateTruncating(_offA);
        var offB = T.CreateTruncating(_offB);
        
        // move (a, b) to (offA, offB)
        Span<T> oa = stackalloc T[a.Length];
        Span<T> ob = stackalloc T[b.Length];
        TensorPrimitives.ConvertTruncating(a, oa);
        TensorPrimitives.ConvertTruncating(b, ob);
        TensorPrimitives.Subtract(oa, offA, oa); // a - offA
        TensorPrimitives.Subtract(ob, offB, ob); // b - offB

        Span<T> oaSin = stackalloc T[a.Length];
        Span<T> oaCos = oa;
        Span<T> obSin = stackalloc T[b.Length];
        Span<T> obCos = ob;
        TensorPrimitives.Multiply(oa, T.CreateTruncating(sin), oaSin);
        TensorPrimitives.Multiply(oa, T.CreateTruncating(cos), oaCos);
        TensorPrimitives.Multiply(ob, T.CreateTruncating(sin), obSin);
        TensorPrimitives.Multiply(ob, T.CreateTruncating(cos), obCos);
        
        TensorPrimitives.Subtract(oaCos, obSin, oa);
        TensorPrimitives.Add(oa, offA, oa);
        
        TensorPrimitives.Add(oaSin, obCos, ob);
        TensorPrimitives.Add(ob, offB, ob);
        
        // Functionally equivalent to:
        // for (var i = 0; i < a.Length; i++)
        // {
        //     var pa = a[i];
        //     var pb = b[i];
        //     var oa = (pa - offA);
        //     var ob = (pb - offB);
        //     var oaCos = oa * cos;
        //     var obSin = ob * sin;
        //     a[i] = offA + (oaCos - obSin);
        //     var oaSin = oa * sin;
        //     var obCos = ob * cos;
        //     b[i] = offB + (oaSin + obCos);
        // }
        
        TensorPrimitives.ConvertTruncating(oa, a);
        TensorPrimitives.ConvertTruncating(ob, b);
    }
}