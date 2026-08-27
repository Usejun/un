using Un.Object;
using Un.Object.Primitive;
using Un.Reflection;

namespace Un.Native;

[BuiltinModule("builtin")]
public static class Builtin
{
    [Native(
        Name = "len",
        Description = "Returns the number of items in a collection.",
        Example = "write(len(items))",
        ReturnType = "integer",
        ArgumentTypes = new[] { "collection" }
    )]
    public static Obj Len([ArgInfo(Essential = true)] Obj value) => value.Len();

    [Native(
        Name = "exit",
        Description = "Stops the current UN program with an exit code.",
        Example = "exit(0)",
        ReturnType = "none",
        ArgumentTypes = new[] { "integer" }
    )]
    public static Obj Exit([ArgInfo(Optional = true)] Obj code)
    {
        Environment.Exit(code.As<Int>(out var codeObj) ? (int)codeObj.Value : 0);
        return Obj.None;
    }

    [Native(
        Name = "type",
        Description = "Returns the UN type of a value.",
        Example = "write(type(value))",
        ReturnType = "type",
        ArgumentTypes = new[] { "any" }
    )]
    public static TObj Type([ArgInfo(Essential = true)] Obj value) => new(value.Type);

    [Native(
        Name = "hash",
        Description = "Returns a hash value for an object.",
        Example = "write(hash(value))",
        ReturnType = "integer",
        ArgumentTypes = new[] { "any" }
    )]
    public static Obj Hash([ArgInfo(Essential = true)] Obj value) => value.Hash();

    [Native(
        Name = "panic",
        Description = "Raises a named runtime error.",
        Example = "panic(\"invalid state\")",
        ReturnType = "error",
        ArgumentTypes = new[] { "string", "string" }
    )]
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
