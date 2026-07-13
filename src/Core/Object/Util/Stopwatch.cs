using Un.Reflection;
using Un.Object.Type;
using Un.Object.Primitive;

namespace Un.Object.Util;

[NativeType(Name = "stopwatch")]
public class Stopwatch() : Ref<System.Diagnostics.Stopwatch>(new(), UnType.Create("stopwatch"))
{
    private long lap = 0;

    [Native(Name = "start")]
    public static Obj Start([Self] Stopwatch self)
    {
        self.Value.Start();
        return None;
    }

    [Native(Name = "stop")]
    public static Obj Stop([Self] Stopwatch self)
    {
        self.Value.Stop();
        return None;
    }

    [Native(Name = "reset")]
    public static Obj Reset([Self] Stopwatch self)
    {
        self.Value.Reset();
        return None;
    }

    [Native(Name = "lap")]
    public static Int Lap([Self] Stopwatch self)
    {
        long now = self.Value.ElapsedTicks;
        long diff = now - self.lap;
        self.lap = now;

        return Int.From(diff);
    }

    [Native(Name = "restart")]
    public static Obj Restart([Self] Stopwatch self)
    {
        self.Value.Restart();
        return None;
    }

    [Native(Name = "is_running")]
    public static Bool IsRunning([Self] Stopwatch self) => Bool.From(self.Value.IsRunning);

    [Native(Name = "tick")]
    public static Int Tick([Self] Stopwatch self) => Int.From(self.Value.ElapsedTicks);

    [Native(Name = "ms")]
    public static Int Ms([Self] Stopwatch self) => Int.From(self.Value.ElapsedMilliseconds);

    [Native(Name = "seconds")]
    public static Float Seconds([Self] Stopwatch self) => new(self.Value.Elapsed.TotalSeconds);

    [Native(Name = "minutes")]
    public static Float Minutes([Self] Stopwatch self) => new(self.Value.Elapsed.TotalMinutes);

    [Native(Name = "hours")]
    public static Float Hours([Self] Stopwatch self) => new(self.Value.Elapsed.TotalHours);

    [Native(Name = "elapsed")]
    public static Date Elapsed([Self] Stopwatch self) => new(new(self.Value.Elapsed.Ticks));
}
