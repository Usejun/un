using Un.Object;
using Un.Object.Primitive;
using Un.Reflection;

namespace Un.Native;

[NativeModule("math")]
public static class Math
{
    [Native(Name = "round")]
    public static Obj Round(
        [ArgInfo(Essential = true)] Obj value,
        [ArgInfo(Optional = true)] Obj digit = null!
    )
    {
        double v = value switch
        {
            Int i => i.Value,
            Float f => f.Value,
            _ => double.NaN,
        };

        if (double.IsNaN(v))
            return new Err("expected number type");

        if (!digit.As<Int>(out var digitObj) || digitObj.Value > 15 || digitObj.Value < 0)
            return new Err("digit is must be int and greater then 0 and less then 15");

        int d = (int)digitObj.Value;

        v = System.Math.Round(v, d);

        return d == 0 || double.IsInteger(v) ? Int.From((long)v) : new Float(v);
    }

    [Native(Name = "abs")]
    public static Obj Abs(
        [ArgInfo(Essential = true)] Obj value
    ) => value switch
    {
        Int i => Int.From(System.Math.Abs(i.Value)),
        Float f => new Float(System.Math.Abs(f.Value)),
        _ => new Err("expected number type")
    };

    [Native(Name = "ceil")]
    public static Obj Ceil(
        [ArgInfo(Essential = true)] Obj value
    ) => value switch
    {
        Int i => Int.From(i.Value),
        Float f => new Float(System.Math.Ceiling(f.Value)),
        _ => new Err("expected number type")
    };

    [Native(Name = "sqrt")]
    public static Obj Sqrt(
        [ArgInfo(Essential = true)] Obj value
    ) => value switch
    {
        Int i => new Float(System.Math.Sqrt(i.Value)),
        Float f => new Float(System.Math.Sqrt(f.Value)),
        _ => new Err("expected number type")
    };

    [Native(Name = "floor")]
    public static Obj Floor(
        [ArgInfo(Essential = true)] Obj value
    ) => value switch
    {
        Int i => Int.From(i.Value),
        Float f => new Float(System.Math.Floor(f.Value)),
        _ => new Err("expected number type")
    };
}
