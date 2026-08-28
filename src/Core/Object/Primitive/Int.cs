using Un.Object.Collections;
using Un.Object.Type;
using Un.Reflection;

namespace Un.Object.Primitive;

[BuiltinType("int")]
public class Int : Val<long>
{
    private readonly static Int[] caches = [.. Enumerable.Range(-5, 262).Select(i => new Int(i))];

    public Int() : this(0) {}

    private Int(long value) : base(value, UnType.Int) { }

    public override Obj Init(Tup args) => args switch
    {
        { Count: 0 } => From(0),
        { Count: 1 } => args[0].ToInt(),
        _ => new Err($"cannot convert to '{Type}'"),
    };

    public override Obj Add(Obj other) => other switch
    {
        Int i => From(Value + i.Value),
        Float f => new Float(Value + f.Value),
        Str s => Str.From(Value.ToString() + s.Value),
        _ => new Err($"unsupported operand type(s) for +: 'int' and '{other.Type}'")
    };

    public override Obj Sub(Obj other) => other switch
    {
        Int i => From(Value - i.Value),
        Float f => new Float(Value - f.Value),
        _ => new Err($"unsupported operand type(s) for -: 'int' and '{other.Type}'")
    };

    public override Obj Mul(Obj other) => other switch
    {
        Int i => From(Value * i.Value),
        Float f => new Float(Value * f.Value),
        _ => new Err($"unsupported operand type(s) for *: 'int' and '{other.Type}'")
    };

    public override Obj Div(Obj other) => other switch
    {
        Int i => i.Value == 0 ? new Err($"{Type} division by zero") : new Float((double)Value / i.Value),
        Float f => f.Value == 0 ? new Err($"{Type} division by zero") : new Float(Value / f.Value),
        _ => new Err($"unsupported operand type(s) for /: 'int' and '{other.Type}'")
    };

    public override Obj Mod(Obj other) => other switch
    {
        Int i => From(Value % i.Value),
        Float f => new Float(Value % f.Value),
        _ => new Err($"unsupported operand type(s) for %: 'int' and '{other.Type}'")
    };

    public override Obj IDiv(Obj other) => other switch
    {
        Int i => i.Value == 0 ? new Err($"{Type} division by zero") : From(Value / i.Value),
        Float f => f.Value == 0 ? new Err($"{Type} division by zero") : From((long)(Value / f.Value)),
        _ => new Err($"unsupported operand type(s) for //: 'int' and '{other.Type}'")
    };

    public override Obj Pow(Obj other) => other switch
    {
        Int i => From((long)Math.Pow(Value, i.Value)),
        Float f => new Float(Math.Pow(Value, f.Value)),
        _ => new Err($"unsupported operand type(s) for **: 'int' and '{other.Type}'")
    };

    public override Int Neg() => From(-Value);

    public override Int Pos() => From(+Value);

    public override Obj BAnd(Obj other) => other switch
    {
        Int i => From(Value & i.Value),
        _ => new Err($"unsupported operand type(s) for &: 'int' and '{other.Type}'")
    };

    public override Obj BOr(Obj other) => other switch
    {
        Int i => From(Value | i.Value),
        _ => new Err($"unsupported operand type(s) for |: 'int' and '{other.Type}'")
    };

    public override Obj BXor(Obj other) => other switch
    {
        Int i => From(Value ^ i.Value),
        _ => new Err($"unsupported operand type(s) for ^: 'int' and '{other.Type}'")
    };

    public override Int BNot() => From(~Value);

    public override Obj LShift(Obj other) => other switch
    {
        Int i => From(Value << (int)i.Value),
        _ => new Err($"unsupported operand type(s) for <<: 'int' and '{other.Type}'")
    };

    public override Obj RShift(Obj other) => other switch
    {
        Int i => From(Value >> (int)i.Value),
        _ => new Err($"unsupported operand type(s) for >>: 'int' and '{other.Type}'")
    };

    public override Obj Eq(Obj other) => other switch
    {
        Int i => Bool.From(Value == i.Value),
        Float f => Bool.From(Value == f.Value),
        Obj o when o.IsNone() => Bool.False,
        _ => new Err($"unsupported operand type(s) for ==: 'int' and '{other.Type}'")
    };

    public override Obj Lt(Obj other) => other switch
    {
        Int i => Bool.From(Value < i.Value),
        Float f => Bool.From(Value < f.Value),
        _ => new Err($"unsupported operand type(s) for <: 'int' and '{other.Type}'")
    };

    public override Obj Gt(Obj other) => other switch
    {
        Int i => Bool.From(Value > i.Value),
        Float f => Bool.From(Value > f.Value),
        _ => new Err($"unsupported operand type(s) for >: 'int' and '{other.Type}'")
    };

    public override Obj LtOrEq(Obj other) => other switch
    {
        Int i => Bool.From(Value <= i.Value),
        Float f => Bool.From(Value <= f.Value),
        _ => new Err($"unsupported operand type(s) for <=: 'int' and '{other.Type}'")
    };

    public override Obj GtOrEq(Obj other) => other switch
    {
        Int i => Bool.From(Value >= i.Value),
        Float f => Bool.From(Value >= f.Value),
        _ => new Err($"unsupported operand type(s) for >=: 'int' and '{other.Type}'")
    };

    public override Obj NEq(Obj other) => other switch
    {
        Int i => Bool.From(Value != i.Value),
        Float f => Bool.From(Value != f.Value),
        Obj o when o.IsNone() => Bool.True,
        _ => new Err($"unsupported operand type(s) for !=: 'int' and '{other.Type}'")
    };

    public override Int Len() => From(1);

    public override Int ToInt() => From(Value);

    public override Float ToFloat() => new(Value);

    public override Str ToStr() => Str.From(Value.ToString());

    public override Bool ToBool() => Bool.From(Value != 0);

    public override Int Copy() => From(Value);

    public override Int Clone() => From(Value);

    public static Int From(long value)
    {
        if (value >= -5 && value <= 256)
            return caches[value + 5];
        return new Int(value);
    }
}