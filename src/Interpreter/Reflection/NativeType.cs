namespace Un.Reflection;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class NativeTypeAttribute : Attribute
{
    public string? Name { get; init; }
}