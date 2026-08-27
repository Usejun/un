using Un.Object.Primitive;
using Un.Object.Collections;
using System.Collections.Concurrent;
using Un.Object.Function;
using Un.Object.Type;
using Un.Reflection;

namespace Un.Object.Flow;

[NativeType(Name = "pool")]
public class Pool : Obj
{
    private readonly BlockingCollection<Future> queue = [];
    private readonly List<Thread> threads = [];

    public Pool() : this(4) { }

    public Pool(long workers) : base(UnType.Create("pool"))
    {
        for (int i = 0; i < workers; i++)
        {
            var thread = new Thread(() =>
            {
                foreach (var future in queue.GetConsumingEnumerable())
                    future.Run();
            });
            thread.Start();
            threads.Add(thread);
        }
    }

    public override Obj Init(Tup args) => args switch
    {
        { Count: 0 } => new Pool(4),
        { Count: 1 } => args[0] is Int count ? new Pool(count.Value) : new Err("expected a worker count as an integer"),
        _ => new Err("pool takes at most one argument")
    };

    public override Obj Entry() => this;

    public override Obj Exit()
    {
        queue.CompleteAdding();
        foreach (var thread in threads)
            thread.Join();
        return None;
    }

    public override Obj Copy() => this;

    public override Obj Clone() => new Err("'lock' cannot be cloned");

    [Native(
        Name = "submit",
        Description = "Returns the result of workers.submit().",
        Example = "workers.submit(fn, args)",
        ReturnType = "any",
        ArgumentTypes = new[] { "any", "any" }
    )]
    public static Obj Submit(
        [Self] Pool self,
        [ArgInfo(Essential = true)] Obj fn,
        [ArgInfo(Positional = true)] Obj args)
    {
        if (!fn.As<Fn>(out _))
            return new Err("expected 'fn' argument to be of type 'func'");

        if (!args.As<Tup>(out var argsTup))
            return new Err("expected 'args' argument to be of type 'collection'");

        var future = new Future(new Task<Obj>(() => fn.Call(argsTup)));
        self.queue.Add(future);
        return future;
    }

    [Native(
        Name = "map",
        Description = "Returns the result of workers.map().",
        Example = "workers.map(fn, vargs)",
        ReturnType = "list",
        ArgumentTypes = new[] { "any", "any" }
    )]
    public static Obj Map(
        [Self] Pool self,
        [ArgInfo(Essential = true)] Obj fn,
        [ArgInfo(Positional = true)] Obj vargs)
    {
        if (!fn.As<Fn>(out _))
            return new Err("expected 'fn' argument to be of type 'func'");

        if (vargs.As<Tup>(out var vargsTup))
            return new Err("expected 'iterable' argument to be of type 'iterable'");

        var len = vargsTup.Count;
        var result = new List<Obj>();
        var countdown = new CountdownEvent(len);

        foreach (var varg in vargsTup.Value)
        {
            self.queue.Add(new Future(new Task<Obj>(() =>
            {
                var res = fn.Call(varg is Tup t ? t : new([varg]));
                lock (result)
                {
                    result.Add(res);
                }
                countdown.Signal();
                return None;
            })));
        }

        countdown.Wait();

        return new List([.. result]);
    }

    [Native(
        Name = "close",
        Description = "Closes workers.",
        Example = "workers.close()",
        ReturnType = "none"
    )]
    public static Obj Close([Self] Pool self)
    {
        self.queue.CompleteAdding();
        foreach (var thread in self.threads)
            thread.Join();
        return None;
    }
}
