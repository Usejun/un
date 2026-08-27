using Un.Object;
using Un.Object.Primitive;
using Un.Reflection;

namespace Un.Native;

[NativeModule("math")]
public static class Math
{
    const int MaxDigits = 15;

    static bool GetNumber(Obj obj, out double value, out Obj err)
    {
        switch (obj)
        {
            case Int i:
                value = i.Value;
                err = Obj.None;
                return true;
            case Float f:
                value = f.Value;
                err = Obj.None;
                return true;
            default:
                value = double.NaN;
                err = new Err("expected number type");
                return false;
        }
    }

    static bool GetInt(Obj obj, out long value, out Obj err)
    {
        if (!obj.As<Int>(out var i))
        {
            value = 0;
            err = new Err("expected integer type");
            return false;
        }

        value = i.Value;
        err = Obj.None;
        return true;
    }

    static Obj Wrap(Obj original, double value)
    {
        return original is Int ? Int.From((long)value) : new Float(value);
    }

    [Native(Name = "round")]
    public static Obj Round(
        [ArgInfo(Essential = true)] Obj value,
        [ArgInfo(Optional = true)] Obj digit = null!)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        int d = 0;

        if (digit != null)
        {
            if (!digit.As<Int>(out var digitObj) || digitObj.Value > MaxDigits || digitObj.Value < 0)
                return new Err("digit must be an integer between 0 and 15");

            d = (int)digitObj.Value;
        }

        v = System.Math.Round(v, d);

        return d == 0 || double.IsInteger(v) ? Int.From((long)v) : new Float(v);
    }

    [Native(Name = "abs")]
    public static Obj Abs([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return Wrap(value, System.Math.Abs(v));
    }

    [Native(Name = "ceil")]
    public static Obj Ceil([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return value is Int ? value : new Float(System.Math.Ceiling(v));
    }

    [Native(Name = "floor")]
    public static Obj Floor([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return value is Int ? value : new Float(System.Math.Floor(v));
    }

    [Native(Name = "trunc")]
    public static Obj Trunc([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return value is Int ? value : Int.From((long)System.Math.Truncate(v));
    }

    [Native(Name = "sign")]
    public static Obj Sign([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return Int.From(System.Math.Sign(v));
    }

    [Native(Name = "sqrt")]
    public static Obj Sqrt([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        if (v < 0)
            return new Err("cannot compute sqrt of a negative number");

        return new Float(System.Math.Sqrt(v));
    }

    [Native(Name = "cbrt")]
    public static Obj Cbrt([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return new Float(System.Math.Cbrt(v));
    }

    [Native(Name = "pow")]
    public static Obj Pow(
        [ArgInfo(Essential = true)] Obj value,
        [ArgInfo(Essential = true)] Obj exponent)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        if (!GetNumber(exponent, out var e, out err))
            return err;

        double result = System.Math.Pow(v, e);

        return value is Int && exponent is Int && e >= 0 && double.IsInteger(result)
            ? Int.From((long)result)
            : new Float(result);
    }

    [Native(Name = "exp")]
    public static Obj Exp([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return new Float(System.Math.Exp(v));
    }

    [Native(Name = "log")]
    public static Obj Log(
        [ArgInfo(Essential = true)] Obj value,
        [ArgInfo(Optional = true)] Obj @base = null!)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        if (v <= 0)
            return new Err("expected value greater than 0");

        if (@base == null)
            return new Float(System.Math.Log(v));

        if (!GetNumber(@base, out var b, out err))
            return err;

        return new Float(System.Math.Log(v, b));
    }

    [Native(Name = "log2")]
    public static Obj Log2([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        if (v <= 0)
            return new Err("expected value greater than 0");

        return new Float(System.Math.Log2(v));
    }

    [Native(Name = "log10")]
    public static Obj Log10([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        if (v <= 0)
            return new Err("expected value greater than 0");

        return new Float(System.Math.Log10(v));
    }

    [Native(Name = "sin")]
    public static Obj Sin([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return new Float(System.Math.Sin(v));
    }

    [Native(Name = "cos")]
    public static Obj Cos([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return new Float(System.Math.Cos(v));
    }

    [Native(Name = "tan")]
    public static Obj Tan([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return new Float(System.Math.Tan(v));
    }

    [Native(Name = "asin")]
    public static Obj Asin([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return new Float(System.Math.Asin(v));
    }

    [Native(Name = "acos")]
    public static Obj Acos([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return new Float(System.Math.Acos(v));
    }

    [Native(Name = "atan")]
    public static Obj Atan([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return new Float(System.Math.Atan(v));
    }

    [Native(Name = "atan2")]
    public static Obj Atan2(
        [ArgInfo(Essential = true)] Obj y,
        [ArgInfo(Essential = true)] Obj x)
    {
        if (!GetNumber(y, out var yv, out var err))
            return err;

        if (!GetNumber(x, out var xv, out err))
            return err;

        return new Float(System.Math.Atan2(yv, xv));
    }

    [Native(Name = "hypot")]
    public static Obj Hypot(
        [ArgInfo(Essential = true)] Obj x,
        [ArgInfo(Essential = true)] Obj y)
    {
        if (!GetNumber(x, out var xv, out var err))
            return err;

        if (!GetNumber(y, out var yv, out err))
            return err;

        return new Float(System.Math.Sqrt(xv * xv + yv * yv));
    }

    [Native(Name = "clamp")]
    public static Obj Clamp(
        [ArgInfo(Essential = true)] Obj value,
        [ArgInfo(Essential = true)] Obj min,
        [ArgInfo(Essential = true)] Obj max)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        if (!GetNumber(min, out var lo, out err))
            return err;

        if (!GetNumber(max, out var hi, out err))
            return err;

        if (lo > hi)
            return new Err("min must be less than or equal to max");

        return Wrap(value, System.Math.Clamp(v, lo, hi));
    }

    [Native(Name = "gcd")]
    public static Obj Gcd(
        [ArgInfo(Essential = true)] Obj a,
        [ArgInfo(Essential = true)] Obj b)
    {
        if (!GetInt(a, out var av, out var err))
            return err;

        if (!GetInt(b, out var bv, out err))
            return err;

        long x = System.Math.Abs(av);
        long y = System.Math.Abs(bv);

        while (y != 0)
            (x, y) = (y, x % y);

        return Int.From(x);
    }

    [Native(Name = "lcm")]
    public static Obj Lcm(
        [ArgInfo(Essential = true)] Obj a,
        [ArgInfo(Essential = true)] Obj b)
    {
        if (!GetInt(a, out var av, out var err))
            return err;

        if (!GetInt(b, out var bv, out err))
            return err;

        if (av == 0 || bv == 0)
            return Int.From(0);

        long x = System.Math.Abs(av);
        long y = System.Math.Abs(bv);
        long gcd = x;
        long rem = y;

        while (rem != 0)
            (gcd, rem) = (rem, gcd % rem);

        return Int.From(x / gcd * y);
    }

    [Native(Name = "is_nan")]
    public static Obj IsNan([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return Bool.From(double.IsNaN(v));
    }

    [Native(Name = "is_infinite")]
    public static Obj IsInfinite([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return Bool.From(double.IsInfinity(v));
    }

    [Native(Name = "is_finite")]
    public static Obj IsFinite([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return Bool.From(double.IsFinite(v));
    }

    [Native(Name = "pi")]
    public static Obj Pi() => new Float(System.Math.PI);

    [Native(Name = "e")]
    public static Obj E() => new Float(System.Math.E);

    [Native(Name = "tau")]
    public static Obj Tau() => new Float(System.Math.Tau);
}