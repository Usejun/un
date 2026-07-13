using Un.Object.Collections;
using Un.Object.Iter;
using Un.Object.Type;
using Un.Reflection;

namespace Un.Object.Primitive;

[BuiltinType("str")]
public class Str : Ref<string>
{   
    public static readonly Str Empty = new();
    private static Dictionary<string, Str> pool = [];

    public Str() : this("") { }
    
    private Str(string value) : base(value, UnType.Str) { }

    public override Obj Init(Tup args) => args switch
    {
        { Count: 0 } => From(""),
        { Count: 1 } => args[0].ToStr(),
        { Count: 2 } and [Str str, Int i] => From(string.Concat(Enumerable.Repeat(str.Value, (int)Math.Min(i.Value, int.MaxValue)))),
        _ => new Err($"too many arguments"),
    };

    public char this[int index] => Value[index];

    public override Obj Add(Obj other)
    {
        var str = other.ToStr();

        if (str is Err err)
            return err;

        if (!str.As<Str>(out var strValue))
            return new Err($"cannot add '{other.Type}' to '{Type}'");

        return From(Value + strValue.Value);
    }

    public override Obj Sub(Obj other)
    {
        var str = other.ToStr();

        if (str is Err err)
            return err;

        if (!str.As<Str>(out var strValue))
            return new Err($"cannot add '{other.Type}' to '{Type}'");

        return From(Value.Replace(strValue.Value, ""));
    }

    public override Obj Eq(Obj other) => other switch
    {
        Str s => Bool.From(Value.CompareTo(s.Value) == 0),
        Obj o when o.IsNone() => Bool.False,
        _ => new Err($"unsupported operand type(s) for ==: '{Type}' and '{other.Type}'")
    };

    public override Obj NEq(Obj other) => other switch
    {
        Str s => Bool.From(Value.CompareTo(s.Value) != 0),
        Obj o when o.IsNone() => Bool.True,
        _ => new Err($"unsupported operand type(s) for ==: '{Type}' and '{other.Type}'")
    };

    public override Obj Lt(Obj other) => other switch
    {
        Str s => Bool.From(Value.CompareTo(s.Value) < 0),
        _ => new Err($"unsupported operand type(s) for <: '{Type}' and '{other.Type}'")
    };

    public override Obj GetItem(Obj other) => other switch
    {
        Int i => OutOfRange((int)i.Value) ? OutOfRange((int)(i.Value + Value.Length)) ? new Err("list index out of range") : 
        Str.From($"{this[(int)(i.Value + Value.Length)]}") : Str.From($"{this[(int)i.Value]}"),
        _ => new Err("invalid index type"),
    };

    public override Int Len() => Int.From(Value.Length);

    public override Obj ToInt() => long.TryParse(Value, out var result) ? Int.From(result) : new Err($"cannot convert '{Value}' to 'int'");

    public override Obj ToFloat() => double.TryParse(Value, out var result) ? new Float(result) : new Err($"cannot convert '{Value}' to 'float'");

    public override Str ToStr() => this;

    public override Bool ToBool() => bool.TryParse(Value, out var result) ? result ? Bool.True : Bool.False : string.IsNullOrEmpty(Value) ? Bool.False : Bool.True;

    public override List ToList()
    {
        var list = new List();
        foreach (var c in Value)
            list.Add(From($"{c}"));
        return list;
    }

    public override Tup ToTuple() => ToList().ToTuple();

    private bool OutOfRange(int value)
    {
        if (Value.Length <= value)
            return true;
        return false;
    }

    public override Str Copy() => new(Value);

    public override Str Clone() => new(Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public override Int Hash() => Int.From(GetHashCode());

    public static Str From(string value)
    {
        if (value == null) return Empty;

        if (value.Length > 32)
            return new Str(value);

        if (pool.TryGetValue(value, out var cached))
            return cached;

        var result = new Str(value);
        pool[value] = result;

        return result;
    }

    public static Str From(string value, bool intern)
    {
        if (value == null) return Empty;

        if (!intern) return From(value);

        if (pool.TryGetValue(value, out var cached))
            return cached;

        var result = From(value);
        pool[value] = result;

        return result;
    }

    public static Str To(Obj obj)
    {
        if (obj.As<Str>(out var str)) return str;
        if (obj.ToStr().As<Str>(out str)) return str;
        return obj.Repr();
    }

    [Native(Name = "is_empty")]
    public static Obj IsEmpty([Self] Str self) => Bool.From(string.IsNullOrEmpty(self.Value));

    [Native(Name = "is_number")]
    public static Obj IsNumber([Self] Str self) => Bool.From(self.Value.All(char.IsDigit));

    [Native(Name = "is_alphabet")]
    public static Obj IsAlphabet([Self] Str self) => Bool.From(self.Value.All(char.IsLetter));

    [Native(Name = "index_of")]
    public static Obj IndexOf([Self] Str self, [ArgInfo(Essential = true)] Obj value)
    {
        if (value.As<Str>(out var str))
            return new Err("invalid argument: value");

        return Int.From(self.Value.IndexOf(str.Value));
    }

    [Native(Name = "contains")]
    public static Obj Contains([Self] Str self, [ArgInfo(Essential = true)] Obj value)
    {
        if (value.As<Str>(out var str))
            return new Err("invalid argument: value");

        return Bool.From(self.Value.Contains(str.Value));
    }

    [Native(Name = "starts_with")]
    public static Obj StartsWith([Self] Str self, [ArgInfo(Essential = true)] Obj value)
    {
        if (value.As<Str>(out var str))
            return new Err("invalid argument: value");

        return Bool.From(self.Value.StartsWith(str.Value));
    }

    [Native(Name = "ends_with")]
    public static Obj EndsWith([Self] Str self, [ArgInfo(Essential = true)] Obj value)
    {
        if (value.As<Str>(out var str))
            return new Err("invalid argument: value");

        return Bool.From(self.Value.EndsWith(str.Value));
    }

    [Native(Name = "to_upper")]
    public static Obj ToUpper([Self] Str self) => From(self.Value.ToUpper());

    [Native(Name = "to_lower")]
    public static Obj ToLower([Self] Str self) => From(self.Value.ToLower());

    [Native(Name = "split")]
    public static Obj Split([Self] Str self, [ArgInfo(Essential = true)] Obj sep)
    {
        if (sep.As<Str>(out var str))
            return new Err("invalid argument: value");

        var parts = self.Value.Split(str.Value);
        return new List([.. parts.Select(From)]);
    }

    [Native(Name = "trim")]
    public static Obj Trim([Self] Str self, [ArgInfo(Essential = true)] Obj chars)
    {
        if (chars.As<Str>(out var str))
            return new Err("invalid argument: value");

        return From(self.Value.Trim(str.Value.ToCharArray()));
    }

    [Native(Name = "join")]
    public static Obj Join([Self] Str self, [ArgInfo(Essential = true)] Obj values)
    {
        if (values.Iter().As<Iters>(out var str))
            return new Err("invalid argument: values");

        var strs = new List<string>();

        foreach (var item in str.Value)
        {
            var s = item.ToStr();
            if (s is Err err)
                return err;

            if (!str.As<Str>(out var strValue))
                return new Err($"cannot join '{item.Type}' with '{self.Type}'");

            strs.Add(strValue.Value);
        }

        return From(string.Join(self.Value, strs));
    }

    [Native(Name = "center")]
    public static Obj Center(
        [Self] Str self, 
        [ArgInfo(Essential = true)] Obj width,
        [ArgInfo(Optional = true)] Obj fill = null!)
    {
        if (width.As<Int>(out var widthValue))
            return new Err("invalid argument: width");

        fill ??= From(" ");

        if (fill.As<Str>(out var fillValue))
            return new Err("invalid argument: fill");

        var pad = Math.Max(0, widthValue.Value - self.Value.Length);
        var left = pad / 2;
        var right = pad - left;
        var fillChar = fillValue.Value.Length > 0 ? fillValue.Value[0] : ' ';
        return From(new string(fillChar, (int)left) + self.Value + new string(fillChar, (int)right));
    }

    [Native(Name = "left")]
    public static Obj Left(
        [Self] Str self,
        [ArgInfo(Essential = true)] Obj width,
        [ArgInfo(Optional = true)] Obj fill = null!)
    {
        if (width.As<Int>(out var widthValue))
            return new Err("invalid argument: width");

        fill ??= From(" ");

        if (fill.As<Str>(out var fillValue))
            return new Err("invalid argument: fill");

        var pad = Math.Max(0, widthValue.Value - self.Value.Length);
        var fillChar = fillValue.Value.Length > 0 ? fillValue.Value[0] : ' ';
        return From(self.Value + new string(fillChar, (int)pad));
    }

    [Native(Name = "right")]
    public static Obj Right(
       [Self] Str self,
       [ArgInfo(Essential = true)] Obj width,
       [ArgInfo(Optional = true)] Obj fill = null!)
    {
        if (width.As<Int>(out var widthValue))
            return new Err("invalid argument: width");

        fill ??= From(" ");

        if (fill.As<Str>(out var fillValue))
            return new Err("invalid argument: fill");

        var pad = Math.Max(0, widthValue.Value - self.Value.Length);
        var fillChar = fillValue.Value.Length > 0 ? fillValue.Value[0] : ' ';
        return From(new string(fillChar, (int)pad) + self.Value);
    }

    [Native(Name = "replace")]
    public static Obj Replace(
       [Self] Str self,
       [ArgInfo(Essential = true)] Obj oldValue,
       [ArgInfo(Optional = true)] Obj newValue)
    {
        if (oldValue.As<Str>(out var oldStr))
            return new Err("invalid argument: old_value");
        if (newValue.As<Str>(out var newStr))
            return new Err("invalid argument: new_value");

        return From(self.Value.Replace(oldStr.Value, newStr.Value));
    }

    [Native(Name = "find")]
    public static Obj Find([Self] Str self, [ArgInfo(Essential = true)] Obj subStr)
    {
        if (subStr.As<Str>(out var subStrValue))
            return new Err("invalid argument: sub_str");

        return Int.From(self.Value.IndexOf(subStrValue.Value));
    }
}