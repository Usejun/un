using Un.Object.Primitive;
using Un.Object.Collections;
using Un.Object.Type;

namespace Un.Object.Iter;

public class Counter : Iters
{
    protected long current = 0;

    public Counter() : this(0) { }

    public Counter(long start)
    {
        Type = UnType.Create("counter");
        current = start;
        Value = Default(start);
    }

    public override Obj Init(Tup args) => args switch
    {
        { Count: 0 } => new Counter(),
        { Count: 1 } and [Int i] => new Counter(i.Value),
        _ => new Err($"invaild '{Type}' initialize"),
    };

    public override Int Len() => Int.From(long.MaxValue);

    public override Obj ToList() => new Err("counter is infinite");

    public override Obj ToTuple() => new Err("counter is infinite");

    public override Obj ToStr() => new Err("counter is infinite");

    public override Obj Spread() => new Err("counter is infinite");

    public override Counter Clone() => new(current);

    protected static IEnumerable<Obj> Default(long start)
    {
        long i = start;
        while (true)
            yield return Int.From(i++);
    }
}