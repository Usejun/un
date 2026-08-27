using Un.Object;
using Un.Object.Primitive;
using Un.Object.Util;
using Un.Reflection;

namespace Un.Native;

[NativeModule("time", typeof(Stopwatch))]
public static class Time
{
    [Native(
        Name = "sleep",
        Description = "Waits for a number of milliseconds.",
        Example = "sleep(250)",
        ReturnType = "none",
        ArgumentTypes = new[] { "number" }
    )]
    public static Obj Sleep(Obj milliseconds)
    {
        var ms = milliseconds switch
        {
            Int i => (int)i.Value,
            Float f => (int)(f.Value * 1000),
            Obj o when o.ToInt().As<Int>(out var intValue) => (int)intValue.Value,
            _ => -1,
        };

        if (ms == -1)
            return new Err("expected 'time' argument to be of type 'int' or 'float' or convertible to int");

        Thread.Sleep(ms);
        return Obj.None;
    }

    [Native(
        Name = "now",
        Description = "Returns the current date and time.",
        Example = "write(now())",
        ReturnType = "date"
    )]
    public static Date Now() => new(DateTime.Now);

    [Native(
        Name = "stopwatch",
        Description = "Creates a stopwatch object.",
        Example = "timer = stopwatch()",
        ReturnType = "stopwatch"
    )]
    public static Stopwatch Stopwatch() => new();
}
