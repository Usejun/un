using Un.Object.Collections;
using Un.Object.Type;

namespace Un.Object.Iter;

public class Reverse : Iters
{
    public Reverse() : base()
    {
        Type = UnType.Create("reverse");
    }

    public Reverse(IEnumerable<Obj> values) : base(values)
    {
        Type = UnType.Create("reverse");
    }

    public override Obj Init(Tup args) => args switch
    {
        { Count: 1 } when args[0].Iter().As<Iters>(out var iter) => new Reverse(iter.Value.Reverse()),
        _ => new Err($"invalid '{Type}' initialization"),
    };

    public override Reverse Clone() => new(Value);
}