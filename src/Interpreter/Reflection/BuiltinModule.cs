namespace Un.Reflection;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class BuiltinModuleAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}