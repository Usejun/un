using Un.Object.Collections;
using Un.Object.Primitive;
using Un.Object.Type;
using Un.Reflection;

namespace Un.Object.Iter;

[NativeType(Name = "repeat")]
public class Repeat : Iters
{
    private readonly IEnumerable<Obj> source;
    private readonly long count;

    public Repeat(IEnumerable<Obj> objs, long count) : base(Default(objs, count))
    {
        Type = UnType.Create("repeat");
        source = objs;
        this.count = count;
    }

    public override Obj Init(Tup args) => args switch
    {
        { Count: 1 } and [List list] => new Repeat(list, -1),
        { Count: 1 } and [Tup tup] => new Repeat(tup, -1),
        { Count: 2 } and [List list, Int i] => new Repeat(list.Value, i.Value),
        { Count: 2 } and [Tup tup, Int i] => new Repeat(tup.Value, i.Value),
        _ => new Err($"invalid '{Type}' initialize"),
    };

    public override Int Len() => Int.From(source.LongCount() * count);

    public override Obj ToList() => count < 0 ? new Err("repeat is infinite") : new List([..Value]);

    public override Obj ToTuple() => count < 0 ? new Err("repeat is infinite") : new Tup([.. Value]);

    public override Str ToStr() => Repr();

    public override Obj Spread() => count < 0 ? new Err("repeat is infinite") : new Spreads([.. Value]);

    public override Repeat Clone() => new(source, count);

    protected static IEnumerable<Obj> Default(IEnumerable<Obj> objs, long count)
    {
        if (count < 0)
            while (true)
                foreach (var obj in objs)
                    yield return obj;
        else
            for (long i = 0; i < count; i++)
                foreach (var obj in objs)
                    yield return obj;  
    }
}