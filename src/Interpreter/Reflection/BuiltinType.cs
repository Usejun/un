namespace Un.Reflection;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class BuiltinTypeAttribute(string name) : Attribute
{
    public string Name { get; } = name;
    public string? Description { get; set; }
    public string? Example { get; set; }
}