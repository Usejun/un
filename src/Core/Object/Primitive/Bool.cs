using Un.Object.Collections;
using Un.Object.Type;
using Un.Reflection;

namespace Un.Object.Primitive;

[BuiltinType("bool", Description = "Boolean true/false value.", Example = "b = true\nio.write(b and false)")]
public class Bool : Val<bool>
{
    public readonly static Bool True = new(true);
    public readonly static Bool False = new(false);

    public Bool() : this(false) { }
    private Bool(bool value) : base(value, UnType.Bool) { }

    public override Obj Init(Tup args) => args switch
    {
        { Count: 0 } => False,
        { Count: 1 } => args[0].ToBool(),
        _ => False
    };

    public override Bool Not() => Value ? False : True;

    public override Obj Xor(Obj other) => Value ? other.Not() : this;

    public override Bool Eq(Obj other) => other is Bool b && Value == b.Value ? True : False;

    public override Str ToStr() => Str.From(Value ? "true" : "false");

    public override Bool Copy() => From(Value);

    public override Bool Clone() => From(Value);

    public override Bool ToBool() => Value ? True : False;

    public static Bool From(bool value) => value ? True : False;
}