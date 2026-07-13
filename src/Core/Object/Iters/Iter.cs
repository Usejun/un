using Un.Object.Primitive;
using Un.Object.Collections;
using Un.Object.Function;
using Un.Object.Type;
using Un.Reflection;

namespace Un.Object.Iter;

[BuiltinType("iter")]
public class Iters(IEnumerable<Obj> value) : Ref<IEnumerable<Obj>>(value, UnType.Iter)
{
    public Iters() : this([]) { }

    public override Obj Init(Tup args) => args switch
    {
        { Count: 1 } => args[0].Iter(),
        _ => new Err($"invaild '{Type}' initialize"),
    };

    public IEnumerator<Obj> Enumerator { get; private set; } = null!;

    public override Int Len() => Int.From(Value.Count());

    public override Obj Iter() => this;

    public override Obj ToList() => new List([.. Value]);

    public override Obj ToTuple() => new Tup([.. Value]);

    public override Obj ToStr() => Str.From(string.Join(", ", Value.Select(x => Str.To(x).Value)));

    public override Obj Next()
    {
        Enumerator ??= Value.GetEnumerator();

        if (Enumerator.MoveNext())
            return Enumerator.Current;
        return new Err("iteration stopped");
    }

    public override Obj Spread() => new Spreads([.. Value]);

    public override Iters Copy() => this;

    public override Iters Clone() => new(Value);

    [Native(Name = "take")]
    public static Obj Take([Self] Iters self, [ArgInfo(Essential = true)] Obj n)
    {
        if (!n.As<Int>(out var nValue))
            return new Err("invalid argument: 'n'");

        return new Iters(self.Value.Take((int)nValue.Value));
    }

    [Native(Name = "skip")]
    public static Obj Skip([Self] Iters self, [ArgInfo(Essential = true)] Obj n)
    {
        if (!n.As<Int>(out var nValue))
            return new Err("invalid argument: 'n'");

        return new Iters(self.Value.Skip((int)nValue.Value));
    }

    [Native(Name = "count")]
    public static Obj Count([Self] Iters self) => Int.From(self.Value.Count());

    [Native(Name = "to_list")]
    public static Obj ToList([Self] Iters self) => new List([.. self.Value]);

    [Native(Name = "to_tuple")]
    public static Obj ToTuple([Self] Iters self) => new Tup([.. self.Value]);

    [Native(Name = "map")]
    public static Obj Map([Self] Iters self, [ArgInfo(Essential = true)] Obj fn)
    {
        if (fn is not Fn)
            return new Err("invalid argument: 'fn'");

        return new Iters(self.Value.Select(x =>
        {
            var res = fn.Call(new([x], [""]));
            return res;
        }));
    }

    [Native(Name = "filter")]
    public static Obj Filter([Self] Iters self, [ArgInfo(Essential = true)] Obj fn)
    {
        if (fn is not Fn)
            return new Err("invalid argument: 'fn'");

        return new Iters(self.Value.Where(x =>
        {
            var res = fn.Call(new([x], [""]));
            return res.As<Bool>(out var v) && v.Value;
        }));
    }    

    [Native(Name = "any")]
    public static Obj Any([Self] Iters self, [ArgInfo(Essential = true)] Obj fn)
    {
        if (fn is not Fn)
            return new Err("invalid argument: 'fn'");

        bool result = self.Value.Any(x =>
        {
            var res = fn.Call(new([x], [""]));
            return res.As<Bool>(out var v) && v.Value;
        });

        return Bool.From(result);
    }

    [Native(Name = "all")]
    public static Obj All([Self] Iters self, [ArgInfo(Essential = true)] Obj fn)
    {
        if (fn is not Fn)
            return new Err("invalid argument: 'fn'");

        bool result = self.Value.All(x =>
        {
            var res = fn.Call(new([x], [""]));
            return res.As<Bool>(out var v) && v.Value;
        });

        return Bool.From(result);
    }

    [Native(Name = "sum")]
    public static Obj Sum([Self] Iters self)
    {
        Obj sum = null!;

        foreach (var v in self.Value)
            sum = sum is null ? v : sum.Add(v);

        return sum ?? None;
    }

    [Native(Name = "first")]
    public static Obj First([Self] Iters self) => self.Value.FirstOrDefault() ?? None;

    [Native(Name = "last")]
    public static Obj Last([Self] Iters self) => self.Value.LastOrDefault() ?? None;
}