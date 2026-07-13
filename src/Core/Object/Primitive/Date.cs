using Un.Object.Collections;
using Un.Object.Type;
using Un.Reflection;

namespace Un.Object.Primitive;

[BuiltinType("date")]
public class Date(DateTime value) : Val<DateTime>(value, UnType.Date)
{
    public Date() : this(DateTime.Now) { }

    public override Obj Init(Tup args) => args switch
    {
        { Count: 0 } => new Date(),
        { Count: 1 } => args[0] switch
        {
            Str s => DateTime.TryParse(s.Value, out var result) ? new Date(result) : new Err("invalid date str"),
            _ => new Err($"cannot convert '{args[0].Type}' to 'date'"),
        },
        _ => new Err($"cannot convert to 'date'"),
    };

    public override Obj Add(Obj other) => other switch
    {
        Date d => new Date(Value.AddDays(d.Value.Day)),
        Str s => Str.From(Value.ToString("yyyy-MM-dd") + s.Value),
        _ => new Err($"unsupported operand type(s) for +: 'date' and '{other.Type}'")
    };

    public override Obj Sub(Obj other) => other switch
    {
        Date d => new Date(new DateTime(Value.Subtract(d.Value).Ticks)),
        _ => new Err($"unsupported operand type(s) for -: 'date' and '{other.Type}'")
    };

    public override Str ToStr() => Str.From(Value.ToString("yyyy-MM-dd HH:mm:ss.fff"));

    public override Date Copy() => new(Value);

    public override Date Clone() => new(Value);

    [Native(Name = "year")]
    public static Int Year([Self] Date self) => Int.From(self.Value.Year);

    [Native(Name = "month")]
    public static Int Month([Self] Date self) => Int.From(self.Value.Month);

    [Native(Name = "day")]
    public static Int Day([Self] Date self) => Int.From(self.Value.Day);

    [Native(Name = "hour")]
    public static Int Hour([Self] Date self) => Int.From(self.Value.Hour);

    [Native(Name = "minute")]
    public static Int Minute([Self] Date self) => Int.From(self.Value.Minute);

    [Native(Name = "second")]
    public static Int Second([Self] Date self) => Int.From(self.Value.Second);

    [Native(Name = "ms")]
    public static Int Ms([Self] Date self) => Int.From(self.Value.Millisecond);

    [Native(Name = "timestamp")]
    public static Int Timestamp([Self] Date self)
        => Int.From(new DateTimeOffset(self.Value).ToUnixTimeSeconds());

    [Native(Name = "timestamp_ms")]
    public static Int TimestampMs([Self] Date self)
        => Int.From(new DateTimeOffset(self.Value).ToUnixTimeMilliseconds());

    [Native(Name = "format")]
    public static Str Format([Self] Date self, [ArgInfo(Essential = true)] Str format)
        => Str.From(self.Value.ToString(format.Value));

    [Native(Name = "add_years")]
    public static Date AddYears([Self] Date self, [ArgInfo(Essential = true)] Int years)
        => new(self.Value.AddYears((int)years.Value));

    [Native(Name = "add_months")]
    public static Date AddMonths([Self] Date self, [ArgInfo(Essential = true)] Int months)
        => new(self.Value.AddMonths((int)months.Value));

    [Native(Name = "add_days")]
    public static Date AddDays([Self] Date self, [ArgInfo(Essential = true)] Int days)
        => new(self.Value.AddDays(days.Value));

    [Native(Name = "add_hours")]
    public static Date AddHours([Self] Date self, [ArgInfo(Essential = true)] Int hours)
        => new(self.Value.AddHours(hours.Value));

    [Native(Name = "add_minutes")]
    public static Date AddMinutes([Self] Date self, [ArgInfo(Essential = true)] Int minutes)
        => new(self.Value.AddMinutes(minutes.Value));

    [Native(Name = "add_seconds")]
    public static Date AddSeconds([Self] Date self, [ArgInfo(Essential = true)] Int seconds)
        => new(self.Value.AddSeconds(seconds.Value));

    [Native(Name = "add_ms")]
    public static Date AddMilliseconds([Self] Date self, [ArgInfo(Essential = true)] Int milliseconds)
        => new(self.Value.AddMilliseconds(milliseconds.Value));

    [Native(Name = "weekday")]
    public static Int Weekday([Self] Date self) => Int.From((int)self.Value.DayOfWeek);

    [Native(Name = "day_of_year")]
    public static Int DayOfYear([Self] Date self) => Int.From(self.Value.DayOfYear);

    [Native(Name = "days_in_month")]
    public static Int DaysInMonth([Self] Date self) => Int.From(DateTime.DaysInMonth(self.Value.Year, self.Value.Month));

    [Native(Name = "is_leap_year")]
    public static Bool IsLeapYear([Self] Date self) => Bool.From(DateTime.IsLeapYear(self.Value.Year));

    [Native(Name = "date")]
    public static Date DateOnly([Self] Date self) => new(self.Value.Date);

    [Native(Name = "before")]
    public static Bool Before([Self] Date self, [ArgInfo(Essential = true)] Date other) => Bool.From(self.Value < other.Value);

    [Native(Name = "after")]
    public static Bool After([Self] Date self, [ArgInfo(Essential = true)] Date other)
        => Bool.From(self.Value > other.Value);

    [Native(Name = "compare")]
    public static Int Compare([Self] Date self, [ArgInfo(Essential = true)] Date other)
        => Int.From(self.Value.CompareTo(other.Value));

    [Native(Name = "to_utc")]
    public static Date ToUtc([Self] Date self) => new(self.Value.ToUniversalTime());

    [Native(Name = "to_local")]
    public static Date ToLocal([Self] Date self) => new(self.Value.ToLocalTime());
}