namespace Un.Reflection;

[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
public class ArgInfoAttribute : Attribute
{
    public bool Essential { get; init; }
    public bool Positional { get; init; }
    public bool Optional { get; init; }
    public bool Keyword { get; init; }
}