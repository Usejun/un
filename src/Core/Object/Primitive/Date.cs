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

    [Native(
        Name = "year",
        Description = "Returns the result of date.year().",
        Example = "date.year()",
        ReturnType = "int"
    )]
    public static Int Year([Self] Date self) => Int.From(self.Value.Year);

    [Native(
        Name = "month",
        Description = "Returns the result of date.month().",
        Example = "date.month()",
        ReturnType = "int"
    )]
    public static Int Month([Self] Date self) => Int.From(self.Value.Month);

    [Native(
        Name = "day",
        Description = "Returns the result of date.day().",
        Example = "date.day()",
        ReturnType = "int"
    )]
    public static Int Day([Self] Date self) => Int.From(self.Value.Day);

    [Native(
        Name = "hour",
        Description = "Returns the result of date.hour().",
        Example = "date.hour()",
        ReturnType = "int"
    )]
    public static Int Hour([Self] Date self) => Int.From(self.Value.Hour);

    [Native(
        Name = "minute",
        Description = "Returns the result of date.minute().",
        Example = "date.minute()",
        ReturnType = "int"
    )]
    public static Int Minute([Self] Date self) => Int.From(self.Value.Minute);

    [Native(
        Name = "second",
        Description = "Returns the result of date.second().",
        Example = "date.second()",
        ReturnType = "int"
    )]
    public static Int Second([Self] Date self) => Int.From(self.Value.Second);

    [Native(
        Name = "ms",
        Description = "Returns the result of date.ms().",
        Example = "date.ms()",
        ReturnType = "int"
    )]
    public static Int Ms([Self] Date self) => Int.From(self.Value.Millisecond);

    [Native(
        Name = "timestamp",
        Description = "Returns the result of date.timestamp().",
        Example = "date.timestamp()",
        ReturnType = "int"
    )]
    public static Int Timestamp([Self] Date self)
        => Int.From(new DateTimeOffset(self.Value).ToUnixTimeSeconds());

    [Native(
        Name = "timestamp_ms",
        Description = "Returns the result of date.timestamp ms().",
        Example = "date.timestamp_ms()",
        ReturnType = "int"
    )]
    public static Int TimestampMs([Self] Date self)
        => Int.From(new DateTimeOffset(self.Value).ToUnixTimeMilliseconds());

    [Native(
        Name = "format",
        Description = "Returns the result of date.format().",
        Example = "date.format(format)",
        ReturnType = "str",
        ArgumentTypes = new[] { "str" }
    )]
    public static Str Format([Self] Date self, [ArgInfo(Essential = true)] Str format)
        => Str.From(self.Value.ToString(format.Value));

    [Native(
        Name = "add_years",
        Description = "Adds years to a date value.",
        Example = "date.add_years(years)",
        ReturnType = "date",
        ArgumentTypes = new[] { "int" }
    )]
    public static Date AddYears([Self] Date self, [ArgInfo(Essential = true)] Int years)
        => new(self.Value.AddYears((int)years.Value));

    [Native(
        Name = "add_months",
        Description = "Adds months to a date value.",
        Example = "date.add_months(months)",
        ReturnType = "date",
        ArgumentTypes = new[] { "int" }
    )]
    public static Date AddMonths([Self] Date self, [ArgInfo(Essential = true)] Int months)
        => new(self.Value.AddMonths((int)months.Value));

    [Native(
        Name = "add_days",
        Description = "Adds days to a date value.",
        Example = "date.add_days(days)",
        ReturnType = "date",
        ArgumentTypes = new[] { "int" }
    )]
    public static Date AddDays([Self] Date self, [ArgInfo(Essential = true)] Int days)
        => new(self.Value.AddDays(days.Value));

    [Native(
        Name = "add_hours",
        Description = "Adds hours to a date value.",
        Example = "date.add_hours(hours)",
        ReturnType = "date",
        ArgumentTypes = new[] { "int" }
    )]
    public static Date AddHours([Self] Date self, [ArgInfo(Essential = true)] Int hours)
        => new(self.Value.AddHours(hours.Value));

    [Native(
        Name = "add_minutes",
        Description = "Adds minutes to a date value.",
        Example = "date.add_minutes(minutes)",
        ReturnType = "date",
        ArgumentTypes = new[] { "int" }
    )]
    public static Date AddMinutes([Self] Date self, [ArgInfo(Essential = true)] Int minutes)
        => new(self.Value.AddMinutes(minutes.Value));

    [Native(
        Name = "add_seconds",
        Description = "Adds seconds to a date value.",
        Example = "date.add_seconds(seconds)",
        ReturnType = "date",
        ArgumentTypes = new[] { "int" }
    )]
    public static Date AddSeconds([Self] Date self, [ArgInfo(Essential = true)] Int seconds)
        => new(self.Value.AddSeconds(seconds.Value));

    [Native(
        Name = "add_ms",
        Description = "Adds ms to a date value.",
        Example = "date.add_ms(milliseconds)",
        ReturnType = "date",
        ArgumentTypes = new[] { "int" }
    )]
    public static Date AddMilliseconds([Self] Date self, [ArgInfo(Essential = true)] Int milliseconds)
        => new(self.Value.AddMilliseconds(milliseconds.Value));

    [Native(
        Name = "weekday",
        Description = "Returns the result of date.weekday().",
        Example = "date.weekday()",
        ReturnType = "int"
    )]
    public static Int Weekday([Self] Date self) => Int.From((int)self.Value.DayOfWeek);

    [Native(
        Name = "day_of_year",
        Description = "Returns the result of date.day of year().",
        Example = "date.day_of_year()",
        ReturnType = "int"
    )]
    public static Int DayOfYear([Self] Date self) => Int.From(self.Value.DayOfYear);

    [Native(
        Name = "days_in_month",
        Description = "Returns the result of date.days in month().",
        Example = "date.days_in_month()",
        ReturnType = "int"
    )]
    public static Int DaysInMonth([Self] Date self) => Int.From(DateTime.DaysInMonth(self.Value.Year, self.Value.Month));

    [Native(
        Name = "is_leap_year",
        Description = "Checks whether a date value leap year.",
        Example = "date.is_leap_year()",
        ReturnType = "bool"
    )]
    public static Bool IsLeapYear([Self] Date self) => Bool.From(DateTime.IsLeapYear(self.Value.Year));

    [Native(
        Name = "date",
        Description = "Returns the result of date.date().",
        Example = "date.date()",
        ReturnType = "date"
    )]
    public static Date DateOnly([Self] Date self) => new(self.Value.Date);

    [Native(
        Name = "before",
        Description = "Returns the result of date.before().",
        Example = "date.before(other)",
        ReturnType = "bool",
        ArgumentTypes = new[] { "date" }
    )]
    public static Bool Before([Self] Date self, [ArgInfo(Essential = true)] Date other) => Bool.From(self.Value < other.Value);

    [Native(
        Name = "after",
        Description = "Returns the result of date.after().",
        Example = "date.after(other)",
        ReturnType = "bool",
        ArgumentTypes = new[] { "date" }
    )]
    public static Bool After([Self] Date self, [ArgInfo(Essential = true)] Date other)
        => Bool.From(self.Value > other.Value);

    [Native(
        Name = "compare",
        Description = "Returns the result of date.compare().",
        Example = "date.compare(other)",
        ReturnType = "int",
        ArgumentTypes = new[] { "date" }
    )]
    public static Int Compare([Self] Date self, [ArgInfo(Essential = true)] Date other)
        => Int.From(self.Value.CompareTo(other.Value));

    [Native(
        Name = "to_utc",
        Description = "Converts a date value to utc.",
        Example = "date.to_utc()",
        ReturnType = "date"
    )]
    public static Date ToUtc([Self] Date self) => new(self.Value.ToUniversalTime());

    [Native(
        Name = "to_local",
        Description = "Converts a date value to local.",
        Example = "date.to_local()",
        ReturnType = "date"
    )]
    public static Date ToLocal([Self] Date self) => new(self.Value.ToLocalTime());
}