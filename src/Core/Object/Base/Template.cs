using Un.Object;
using Un.Object.Type;
using Un.Reflection;

namespace Un;

[BuiltinType("template")]
public class Template(IReadOnlyList<string> strings, IReadOnlyList<Obj> values) : Obj(UnType.Create("template"))
{
    public Template() : this([], []) { }

    public IReadOnlyList<string> Strings { get; } = strings;
    public IReadOnlyList<Obj> Values { get; } = values;
}
