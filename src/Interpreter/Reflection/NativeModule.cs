namespace Un.Reflection;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class NativeModuleAttribute(string name, params Type[] types) : Attribute
{
    public string Name { get; } = name;
    public Type[] Types { get; } = types;
}