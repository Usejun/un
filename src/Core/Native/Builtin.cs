using Un.Object;
using Un.Object.Primitive;
using Un.Reflection;

namespace Un.Native;

[BuiltinModule("builtin")]
public static class Builtin
{
    [Native(Name = "len")]
    public static Obj Len([ArgInfo(Essential = true)] Obj value) => value.Len();

    [Native(Name = "exit")]
    public static Obj Exit([ArgInfo(Optional = true)] Obj code)
    {
        Environment.Exit(code.As<Int>(out var codeObj) ? (int)codeObj.Value : 0);
        return Obj.None;
    }

    [Native(Name = "type")]
    public static TObj Type([ArgInfo(Essential = true)] Obj value) => new(value.Type);

    [Native(Name = "hash")]
    public static Obj Hash([ArgInfo(Essential = true)] Obj value) => value.Hash();

    [Native(Name = "panic")]
    public static Obj Panic(
        [ArgInfo(Essential = true)] Obj message,
        [ArgInfo(Optional = true)] Obj name = null!)
    {
        if (!message.ToStr().As<Str>(out var messageValue))
            return new Err("cannot convert argument 'message' to a str");

        name ??= Str.From("panic");

        if (!name.ToStr().As<Str>(out var nameValue))
            return new Err("cannot convert argument 'name' to a str");

        return new Err(messageValue.Value, nameValue.Value);
    }
}
