namespace Un.Reflection;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class NativeAttribute : Attribute
{
    public bool Async { get; init; } = false;
    public string? Name { get; init; }
}