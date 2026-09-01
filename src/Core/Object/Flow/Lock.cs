using Un.Object.Primitive;
using Un.Object.Type;
using Un.Reflection;

namespace Un.Object.Flow;

[NativeType(Name = "lock")]
public class Lock : Obj
{
    private readonly object syncRoot = new();
    private readonly ThreadLocal<bool> isHeld = new(() => false);

    public Lock() : base(UnType.Create("lock")) { }

    public override Obj Entry() => this;

    public override Obj Exit()
    {
        if (isHeld.Value)
        {
            Monitor.Exit(syncRoot);
            isHeld.Value = false;
        }
        return None;
    }

    public override Obj Copy() => this;

    public override Obj Clone() => new Err("'lock' cannot be cloned");

    [Native(
        Name = "acquire",
        Description = "Returns the result of guard.acquire().",
        Example = "guard.acquire()",
        ReturnType = "none"
    )]
    public static Obj Acquire([Self] Lock self)
    {
        Monitor.Enter(self.syncRoot);
        self.isHeld.Value = true;
        return self;
    }

    [Native(
        Name = "try_acquire",
        Description = "Returns the result of guard.try acquire().",
        Example = "guard.try_acquire()",
        ReturnType = "bool"
    )]
    public static Obj TryAcquire([Self] Lock self)
    {
        bool success = Monitor.TryEnter(self.syncRoot);
        self.isHeld.Value = success;
        return Bool.From(success);
    }

    [Native(
        Name = "release",
        Description = "Returns the result of guard.release().",
        Example = "guard.release()",
        ReturnType = "none"
    )]
    public static Obj Release([Self] Lock self)
    {
        if (self.isHeld.Value)
        {
            Monitor.Exit(self.syncRoot);
            self.isHeld.Value = false;
        }
        else
        {
            return new Err("lock not held");
        }
        return self;
    }

    [Native(
        Name = "dispose",
        Description = "Returns the result of guard.dispose().",
        Example = "guard.dispose()",
        ReturnType = "none"
    )]
    public static Obj Dispose([Self] Lock self)
    {
        if (self.isHeld.Value)
        {
            Monitor.Exit(self.syncRoot);
            self.isHeld.Value = false;
        }
        return self;
    }
}
