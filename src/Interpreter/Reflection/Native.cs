namespace Un.Reflection;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class NativeAttribute : Attribute
{
    public string? Name { get; init; }
}