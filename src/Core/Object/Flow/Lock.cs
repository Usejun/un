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

    [Native(Name = "acquire")]
    public static Obj Acquire([Self] Lock self)
    {
        Monitor.Enter(self.syncRoot);
        self.isHeld.Value = true;
        return self;
    }

    [Native(Name = "try_acquire")]
    public static Obj TryAcquire([Self] Lock self)
    {
        bool success = Monitor.TryEnter(self.syncRoot);
        self.isHeld.Value = success;
        return Bool.From(success);
    }

    [Native(Name = "release")]
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

    [Native(Name = "dispose")]
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
