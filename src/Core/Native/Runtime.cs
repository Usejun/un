using Un.Object;
using Un.Reflection;

namespace Un.Native;

[NativeModule("runtime")]
public static class Runtime
{
    [Native(Name = "gc")]
    public static Obj GC()
    {
        System.GC.Collect();
        return Obj.None;
    }

    [Native(Name = "breakpoint")]
    public static Obj Breakpoint()
    {
        Console.WriteLine("breakpoint hit. Press Enter to continue...");
        Console.ReadLine();
        return Obj.None;
    }
}
