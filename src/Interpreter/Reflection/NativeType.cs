namespace Un.Reflection;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class NativeTypeAttribute : Attribute
{
    public NativeTypeAttribute() {}
    public NativeTypeAttribute(string name) { Name = name; }
    public string? Name { get; init; }
    public string? Description { get; set; }
    public string? Example { get; set; }
}