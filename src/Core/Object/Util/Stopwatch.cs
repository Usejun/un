using Un.Reflection;
using Un.Object.Type;
using Un.Object.Primitive;

namespace Un.Object.Util;

[NativeType(Name = "stopwatch")]
public class Stopwatch() : Ref<System.Diagnostics.Stopwatch>(new(), UnType.Create("stopwatch"))
{
    private long lap = 0;

    [Native(
        Name = "start",
        Description = "Starts timer.",
        Example = "timer.start()",
        ReturnType = "none"
    )]
    public static Obj Start([Self] Stopwatch self)
    {
        self.Value.Start();
        return None;
    }

    [Native(
        Name = "stop",
        Description = "Stops timer.",
        Example = "timer.stop()",
        ReturnType = "none"
    )]
    public static Obj Stop([Self] Stopwatch self)
    {
        self.Value.Stop();
        return None;
    }

    [Native(
        Name = "reset",
        Description = "Resets timer.",
        Example = "timer.reset()",
        ReturnType = "none"
    )]
    public static Obj Reset([Self] Stopwatch self)
    {
        self.Value.Reset();
        return None;
    }

    [Native(
        Name = "lap",
        Description = "Returns the result of timer.lap().",
        Example = "timer.lap()",
        ReturnType = "int"
    )]
    public static Int Lap([Self] Stopwatch self)
    {
        long now = self.Value.ElapsedTicks;
        long diff = now - self.lap;
        self.lap = now;

        return Int.From(diff);
    }

    [Native(
        Name = "restart",
        Description = "Restarts timer.",
        Example = "timer.restart()",
        ReturnType = "none"
    )]
    public static Obj Restart([Self] Stopwatch self)
    {
        self.Value.Restart();
        return None;
    }

    [Native(
        Name = "is_running",
        Description = "Checks whether a stopwatch value running.",
        Example = "timer.is_running()",
        ReturnType = "bool"
    )]
    public static Bool IsRunning([Self] Stopwatch self) => Bool.From(self.Value.IsRunning);

    [Native(
        Name = "tick",
        Description = "Returns the result of timer.tick().",
        Example = "timer.tick()",
        ReturnType = "int"
    )]
    public static Int Tick([Self] Stopwatch self) => Int.From(self.Value.ElapsedTicks);

    [Native(
        Name = "ms",
        Description = "Returns the result of timer.ms().",
        Example = "timer.ms()",
        ReturnType = "int"
    )]
    public static Int Ms([Self] Stopwatch self) => Int.From(self.Value.ElapsedMilliseconds);

    [Native(
        Name = "seconds",
        Description = "Returns the result of timer.seconds().",
        Example = "timer.seconds()",
        ReturnType = "float"
    )]
    public static Float Seconds([Self] Stopwatch self) => new(self.Value.Elapsed.TotalSeconds);

    [Native(
        Name = "minutes",
        Description = "Returns the result of timer.minutes().",
        Example = "timer.minutes()",
        ReturnType = "float"
    )]
    public static Float Minutes([Self] Stopwatch self) => new(self.Value.Elapsed.TotalMinutes);

    [Native(
        Name = "hours",
        Description = "Returns the result of timer.hours().",
        Example = "timer.hours()",
        ReturnType = "float"
    )]
    public static Float Hours([Self] Stopwatch self) => new(self.Value.Elapsed.TotalHours);

    [Native(
        Name = "elapsed",
        Description = "Returns the result of timer.elapsed().",
        Example = "timer.elapsed()",
        ReturnType = "date"
    )]
    public static Date Elapsed([Self] Stopwatch self) => new(new(self.Value.Elapsed.Ticks));
}
