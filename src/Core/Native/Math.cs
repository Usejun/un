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

    [Native(
        Name = "round",
        Description = "Rounds a number to a number of digits.",
        Example = "write(round(3.14159, 2))",
        ReturnType = "number",
        ArgumentTypes = new[] { "number", "integer" }
    )]
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

    [Native(
        Name = "abs",
        Description = "Returns the absolute value of a number.",
        Example = "write(abs(-5))",
        ReturnType = "number",
        ArgumentTypes = new[] { "number" }
    )]
    public static Obj Abs([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return Wrap(value, System.Math.Abs(v));
    }

    [Native(
        Name = "ceil",
        Description = "Rounds a number upward.",
        Example = "write(ceil(2.1))",
        ReturnType = "integer",
        ArgumentTypes = new[] { "number" }
    )]
    public static Obj Ceil([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return value is Int ? value : new Float(System.Math.Ceiling(v));
    }

    [Native(
        Name = "floor",
        Description = "Rounds a number downward.",
        Example = "write(floor(2.9))",
        ReturnType = "integer",
        ArgumentTypes = new[] { "number" }
    )]
    public static Obj Floor([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return value is Int ? value : new Float(System.Math.Floor(v));
    }

    [Native(
        Name = "trunc",
        Description = "Removes the fractional part of a number.",
        Example = "write(trunc(2.9))",
        ReturnType = "integer",
        ArgumentTypes = new[] { "number" }
    )]
    public static Obj Trunc([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return value is Int ? value : Int.From((long)System.Math.Truncate(v));
    }

    [Native(
        Name = "sign",
        Description = "Returns the sign of a number.",
        Example = "write(sign(-3))",
        ReturnType = "integer",
        ArgumentTypes = new[] { "number" }
    )]
    public static Obj Sign([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return Int.From(System.Math.Sign(v));
    }

    [Native(
        Name = "sqrt",
        Description = "Returns the square root of a number.",
        Example = "write(sqrt(9))",
        ReturnType = "float",
        ArgumentTypes = new[] { "number" }
    )]
    public static Obj Sqrt([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        if (v < 0)
            return new Err("cannot compute sqrt of a negative number");

        return new Float(System.Math.Sqrt(v));
    }

    [Native(
        Name = "cbrt",
        Description = "Returns the cube root of a number.",
        Example = "write(cbrt(27))",
        ReturnType = "float",
        ArgumentTypes = new[] { "number" }
    )]
    public static Obj Cbrt([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return new Float(System.Math.Cbrt(v));
    }

    [Native(
        Name = "pow",
        Description = "Raises a number to an exponent.",
        Example = "write(pow(2, 8))",
        ReturnType = "float",
        ArgumentTypes = new[] { "number", "number" }
    )]
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

    [Native(
        Name = "exp",
        Description = "Returns e raised to a power.",
        Example = "write(exp(2))",
        ReturnType = "float",
        ArgumentTypes = new[] { "number" }
    )]
    public static Obj Exp([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return new Float(System.Math.Exp(v));
    }

    [Native(
        Name = "log",
        Description = "Returns a logarithm with an optional base.",
        Example = "write(log(8, 2))",
        ReturnType = "float",
        ArgumentTypes = new[] { "number", "number" }
    )]
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

    [Native(
        Name = "log2",
        Description = "Returns the base-2 logarithm.",
        Example = "write(log2(8))",
        ReturnType = "float",
        ArgumentTypes = new[] { "number" }
    )]
    public static Obj Log2([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        if (v <= 0)
            return new Err("expected value greater than 0");

        return new Float(System.Math.Log2(v));
    }

    [Native(
        Name = "log10",
        Description = "Returns the base-10 logarithm.",
        Example = "write(log10(100))",
        ReturnType = "float",
        ArgumentTypes = new[] { "number" }
    )]
    public static Obj Log10([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        if (v <= 0)
            return new Err("expected value greater than 0");

        return new Float(System.Math.Log10(v));
    }

    [Native(
        Name = "sin",
        Description = "Returns the sine of an angle.",
        Example = "write(sin(0))",
        ReturnType = "float",
        ArgumentTypes = new[] { "number" }
    )]
    public static Obj Sin([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return new Float(System.Math.Sin(v));
    }

    [Native(
        Name = "cos",
        Description = "Returns the cosine of an angle.",
        Example = "write(cos(0))",
        ReturnType = "float",
        ArgumentTypes = new[] { "number" }
    )]
    public static Obj Cos([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return new Float(System.Math.Cos(v));
    }

    [Native(
        Name = "tan",
        Description = "Returns the tangent of an angle.",
        Example = "write(tan(0))",
        ReturnType = "float",
        ArgumentTypes = new[] { "number" }
    )]
    public static Obj Tan([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return new Float(System.Math.Tan(v));
    }

    [Native(
        Name = "asin",
        Description = "Returns the arcsine of a number.",
        Example = "write(asin(0))",
        ReturnType = "float",
        ArgumentTypes = new[] { "number" }
    )]
    public static Obj Asin([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return new Float(System.Math.Asin(v));
    }

    [Native(
        Name = "acos",
        Description = "Returns the arccosine of a number.",
        Example = "write(acos(0))",
        ReturnType = "float",
        ArgumentTypes = new[] { "number" }
    )]
    public static Obj Acos([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return new Float(System.Math.Acos(v));
    }

    [Native(
        Name = "atan",
        Description = "Returns the arctangent of a number.",
        Example = "write(atan(1))",
        ReturnType = "float",
        ArgumentTypes = new[] { "number" }
    )]
    public static Obj Atan([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return new Float(System.Math.Atan(v));
    }

    [Native(
        Name = "atan2",
        Description = "Returns the angle for y and x coordinates.",
        Example = "write(atan2(y, x))",
        ReturnType = "float",
        ArgumentTypes = new[] { "number", "number" }
    )]
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

    [Native(
        Name = "hypot",
        Description = "Returns the hypotenuse for two coordinates.",
        Example = "write(hypot(3, 4))",
        ReturnType = "float",
        ArgumentTypes = new[] { "number", "number" }
    )]
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

    [Native(
        Name = "clamp",
        Description = "Limits a number to a minimum and maximum.",
        Example = "write(clamp(score, 0, 100))",
        ReturnType = "number",
        ArgumentTypes = new[] { "number", "number", "number" }
    )]
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

    [Native(
        Name = "gcd",
        Description = "Returns the greatest common divisor.",
        Example = "write(gcd(12, 18))",
        ReturnType = "integer",
        ArgumentTypes = new[] { "integer", "integer" }
    )]
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

    [Native(
        Name = "lcm",
        Description = "Returns the least common multiple.",
        Example = "write(lcm(4, 6))",
        ReturnType = "integer",
        ArgumentTypes = new[] { "integer", "integer" }
    )]
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

    [Native(
        Name = "is_nan",
        Description = "Checks whether a number is not-a-number.",
        Example = "write(is_nan(value))",
        ReturnType = "boolean",
        ArgumentTypes = new[] { "number" }
    )]
    public static Obj IsNan([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return Bool.From(double.IsNaN(v));
    }

    [Native(
        Name = "is_infinite",
        Description = "Checks whether a number is infinite.",
        Example = "write(is_infinite(value))",
        ReturnType = "boolean",
        ArgumentTypes = new[] { "number" }
    )]
    public static Obj IsInfinite([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return Bool.From(double.IsInfinity(v));
    }

    [Native(
        Name = "is_finite",
        Description = "Checks whether a number is finite.",
        Example = "write(is_finite(value))",
        ReturnType = "boolean",
        ArgumentTypes = new[] { "number" }
    )]
    public static Obj IsFinite([ArgInfo(Essential = true)] Obj value)
    {
        if (!GetNumber(value, out var v, out var err))
            return err;

        return Bool.From(double.IsFinite(v));
    }

    [Native(
        Name = "pi",
        Description = "Returns the mathematical constant pi.",
        Example = "write(pi())",
        ReturnType = "float"
    )]
    public static Obj Pi() => new Float(System.Math.PI);

    [Native(
        Name = "e",
        Description = "Returns Euler's number.",
        Example = "write(e())",
        ReturnType = "float"
    )]
    public static Obj E() => new Float(System.Math.E);

    [Native(
        Name = "tau",
        Description = "Returns the mathematical constant tau.",
        Example = "write(tau())",
        ReturnType = "float"
    )]
    public static Obj Tau() => new Float(System.Math.Tau);
}
