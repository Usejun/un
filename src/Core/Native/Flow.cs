using Un.Object;
using Un.Object.Flow;
using Un.Object.Primitive;
using Un.Reflection;

namespace Un.Native;

[NativeModule("flow", typeof(Pool), typeof(Lock))]
public static class Flow
{
    [Native(
        Name = "spawn",
        Description = "Creates a worker pool with an optional worker count.",
        Example = "workers = spawn(4)",
        ReturnType = "pool",
        ArgumentTypes = new[] { "int" }
    )]
    public static Obj Spawn([ArgInfo(Optional = true)] Obj workerCount = null!)
    {
        workerCount ??= Int.From(4);

        if (!workerCount.As<Int>(out var worker))
            return new Err($"argument 'worker' must be of type int");

        return new Pool(worker.Value);
    }

    [Native(
        Name = "lock",
        Description = "Creates a lock for coordinating concurrent work.",
        Example = "guard = lock()",
        ReturnType = "lock"
    )]
    public static Lock Lock() => new();
}
