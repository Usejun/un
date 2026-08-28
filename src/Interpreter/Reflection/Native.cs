using Un.Object.Type;

namespace Un.Reflection;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class NativeAttribute : Attribute
{
    public bool Async { get; init; } = false;
    public string? Name { get; init; }
    public string? Description { get; init; }
    public string? Example { get; init; }
    public string? ReturnType { get; init; }
    public string[] ArgumentTypes { get; init; } = [];
}
