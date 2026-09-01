using Un.Object;
using Un.Object.Primitive;
using Un.Object.Collections;
using Un.Reflection;

namespace Un.Native;

[NativeModule("inspect")]
public static class Inspect
{
    [Native(
        Name = "attr",
        Description = "Lists available attributes for a value.",
        Example = "write(attr(value))",
        ReturnType = "list",
        ArgumentTypes = new[] { "any" }
    )]
    public static List Attr([ArgInfo(Essential = true)] Obj value)
        => new([.. Un.Global.GetAllAttrKeys(value).Select(Str.From)]);

    [Native(
        Name = "global",
        Description = "Returns the global namespace object.",
        Example = "write(global())",
        ReturnType = "dict"
    )]
    public static Dict Global()
    {
        var dict = new Dict();
        foreach (var kv in Un.Global.GetGlobalScope().GetSymbolTable().Keys)
            dict.Value[Str.From(kv)] = Un.Global.GetGlobalVariable(kv);

        return dict;
    }

    [Native(
        Name = "getattr",
        Description = "Reads an attribute with an optional default.",
        Example = "name = getattr(user, \"name\", \"guest\")",
        ReturnType = "any",
        ArgumentTypes = new[] { "any", "str", "any" }
    )]
    public static Obj GetAttr(
        [ArgInfo(Essential = true)] Obj value,
        [ArgInfo(Essential = true)] Obj name,
        [ArgInfo(Optional = true, Name = "default_value")] Obj defaultValue = null!)
    {
        if (!name.ToStr().As<Str>(out var nameValue))
            return new Err("expected 'name' argument to be of type 'str'");
        var result = Un.Global.GetAttrDeep(value, nameValue.Value);
        if (result is Err)
            return defaultValue ?? result;
        return result;
    }

    [Native(
        Name = "hasattr",
        Description = "Checks whether an object has an attribute.",
        Example = "write(hasattr(user, \"name\"))",
        ReturnType = "bool",
        ArgumentTypes = new[] { "any", "str" }
    )]
    public static Obj HasAttr(
        [ArgInfo(Essential = true)] Obj value,
        [ArgInfo(Essential = true)] Obj name)
    {
        if (!name.ToStr().As<Str>(out var nameValue))
            return new Err("expected 'name' argument to be of type 'str'");
        return Bool.From(Un.Global.HasAttrDeep(value, nameValue.Value));
    }

    [Native(
        Name = "setattr",
        Description = "Sets an attribute on an object.",
        Example = "setattr(user, \"name\", \"Ada\")",
        ReturnType = "any",
        ArgumentTypes = new[] { "object", "str", "any" }
    )]
    public static Obj SetAttr(
        [ArgInfo(Essential = true)] Obj value,
        [ArgInfo(Essential = true)] Obj name,
        [ArgInfo(Essential = true, Name = "new_value")] Obj newValue)
    {
        if (!name.ToStr().As<Str>(out var nameValue))
            return new Err("expected 'name' argument to be of type 'str'");
        value.SetAttr(nameValue.Value, newValue);
        return newValue;
    }

    [Native(
        Name = "delattr",
        Description = "Deletes an attribute from an object.",
        Example = "delattr(user, \"name\")",
        ReturnType = "none",
        ArgumentTypes = new[] { "object", "str" }
    )]
    public static Obj DelAttr(
        [ArgInfo(Essential = true)] Obj value,
        [ArgInfo(Essential = true)] Obj name)
    {
        if (!name.ToStr().As<Str>(out var nameValue))
            return new Err("expected 'name' argument to be of type 'str'");
        if (!value.Members.Remove(nameValue.Value))
            return new Err($"attribute '{nameValue.Value}' not found");
        return Obj.None;
    }

    [Native(
        Name = "meta",
        Description = "Reads metadata from an object.",
        Example = "write(meta(value, \"type\"))",
        ReturnType = "any",
        ArgumentTypes = new[] { "object", "str" }
    )]
    public static Obj Meta(
        [ArgInfo(Essential = true)] Obj value,
        [ArgInfo(Essential = true)] Obj name)
    {
        if (!name.ToStr().As<Str>(out var nameValue))
            return new Err("expected 'name' argument to be of type 'str'");
        if (!value.Annotations.TryGetValue(nameValue.Value, out var metaValue))
            return new Err($"attribute '{nameValue.Value}' not found");
        return metaValue;
    }
}
