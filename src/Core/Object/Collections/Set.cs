using Un.Object.Primitive;
using Un.Object.Type;
using Un.Reflection;

namespace Un.Object.Collections;

[BuiltinType("set")]
public class Set(HashSet<Obj> value) : Ref<HashSet<Obj>>(value, UnType.Set)
{
    public Set() : this([]) { }

    public override Obj Init(Tup args) => new Set([..args.ToList()]);

    public override Obj Add(Obj other) => other switch
    {
        Set otherSet => new Set([.. Value.Union(otherSet.Value)]),
        _ => new Err($"unsupported operand type(s) for +: 'set' and '{other.Type}'")
    };

    public override Obj Sub(Obj other)=> other switch
    {
        Set otherSet => new Set([.. Value.Except(otherSet.Value)]),
        _ => new Err($"unsupported operand type(s) for -: 'set' and '{other.Type}'")
    };

    public override Obj BXor(Obj other)=> other switch
    {
        Set otherSet => new Set([.. Value.Intersect(otherSet.Value)]),
        _ => new Err($"unsupported operand type(s) for ^: 'set' and '{other.Type}'")
    };

    public override Int Len() => Int.From(Value.Count);

    public override Obj GetItem(Obj key) => Value.TryGetValue(key, out var value) ? value : new Err($"key {Str.To(key).Value} not found in set");

    public override Set Copy() => this;

    public override Set Clone() => new([.. Value]);

    public override Str ToStr() => Str.From($"{{{string.Join(", ", Value.Select(x => Str.To(x).Value))}}}");

    public override Spreads Spread() => new([.. Value]);

    [Native(Name = "add")]
    public static Obj Add([Self] Set self, [ArgInfo(Essential = true)] Obj value) => Bool.From(self.Value.Add(value));

    [Native(Name = "remove")]
    public static Obj Remove([Self] Set self, [ArgInfo(Essential = true)] Obj value) => Bool.From(self.Value.Remove(value));

    [Native(Name = "contains")]
    public static Obj Contains([Self] Set self, [ArgInfo(Essential = true)] Obj value) => Bool.From(self.Value.Contains(value));

    [Native(Name = "clear")]
    public static Obj Clear([Self] Set self)
    {
        self.Value.Clear();
        return None;
    }

    [Native(Name = "clone")]
    public static Obj Clone([Self] Set self) => self.Clone();

    [Native(Name = "union")]
    public static Obj Union([Self] Set self, [ArgInfo(Essential = true)] Obj other)
    {
        if (!other.As<Set>(out var otherValue))
            return new Err("invalid argument: other");
        return new Set([.. self.Value.Union(otherValue.Value)]);
    }

    [Native(Name = "intersect")]
    public static Obj Intersect([Self] Set self, [ArgInfo(Essential = true)] Obj other)
    {
        if (!other.As<Set>(out var otherValue))
            return new Err("invalid argument: other");
        return new Set([.. self.Value.Intersect(otherValue.Value)]);
    }

    [Native(Name = "difference")]
    public static Obj Difference([Self] Set self, [ArgInfo(Essential = true)] Obj other)
    {
        if (!other.As<Set>(out var otherValue))
            return new Err("invalid argument: other");
        return new Set([.. self.Value.Except(otherValue.Value)]);
    }
}