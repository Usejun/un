global using Attributes = System.Collections.Generic.Dictionary<string, Un.Object.Obj>;
global using Map = System.Collections.Generic.Dictionary<string, Un.Object.Obj>;

using System.Reflection;
using Un.Object;
using Un.Object.Function;
using Un.Object.Primitive;
using Un.Object.Type;
using Un.Reflection;

namespace Un;

public static class Global
{
    public static string PATH { get; private set; } = "";

    public static ulong MAXRECURSIONDEPTH = 1000;

    private static readonly Scope scope = new();
    private static readonly Scope classes = new();
    private static readonly Dictionary<string, Attributes> originalClasses = [];
    private static readonly Dictionary<string, Type> natives = Assembly.GetExecutingAssembly().GetTypes()!
                                                      .Where(t => t.GetCustomAttribute<NativeModuleAttribute>() is not null)
                                                      .ToDictionary(t => t.GetCustomAttribute<NativeModuleAttribute>()!.Name, t => t);

    public static void Init(string path)
    {
        PATH = path;

        Builtin();

        scope.Set("__name__", Str.From("__main__"));
    }

    private static void Builtin()
    {
        var functions = Assembly.GetExecutingAssembly().GetTypes()!.Where(t => t.GetCustomAttribute<BuiltinModuleAttribute>() is not null);

        foreach (var type in functions)
        {
            var module = type.GetCustomAttribute<BuiltinModuleAttribute>()!;

            foreach (var (name, fn) in CreateMethod(type))
                scope.Set(name, fn);
        }

        var primitives = Assembly.GetExecutingAssembly().GetTypes()!.Where(t => t.GetCustomAttribute<BuiltinTypeAttribute>() is not null);

        foreach (var type in primitives)
        {
            var attr = type.GetCustomAttribute<BuiltinTypeAttribute>()!;
            Obj instance = (Obj)Activator.CreateInstance(type)!;
            
            originalClasses.Add(attr.Name, CreateMethod(type));
            classes.Set(attr.Name, instance);
        }
    }

    public static void Include(string name, string? moduleAlias = null, IReadOnlyList<(string Name, string Alias)>? imports = null)
    {
        var map = BuildNativeMap(name);

        ImportMap(map, name, name, moduleAlias, imports);
    }

    public static void Import(string[] path, string? moduleAlias = null, IReadOnlyList<(string Name, string Alias)>? imports = null)
    {
        var map = Load(Path.Combine(PATH, Path.Combine(path)));

        ImportMap(map, string.Join('.', path), path[^1], moduleAlias, imports);
    }

    private static void ImportMap(Map map, string moduleName, string moduleObjectName, string? moduleAlias, IReadOnlyList<(string Name, string Alias)>? imports)
    {
        const string Wildcard = "*";

        if (imports != null)
        {
            if (moduleAlias != null)
            {
                if (scope.ContainsKey(moduleAlias))
                    throw new Panic($"'{moduleAlias}' already exists in the global scope");

                var module = new Obj(UnType.Create(moduleAlias));

                foreach (var (name, alias) in imports)
                {
                    if (name == Wildcard)
                    {
                        foreach (var (k, v) in map)
                        {
                            if (!module.Members.TryAdd(k, v))
                                throw new Panic($"'{k}' already exists in module '{moduleAlias}'");
                        }

                        continue;
                    }

                    if (!map.TryGetValue(name, out var value))
                        throw new Panic($"module '{moduleName}' has no member '{name}'");

                    if (!module.Members.TryAdd(alias, value))
                        throw new Panic($"'{alias}' already exists in module '{moduleAlias}'");
                }

                scope.Set(moduleAlias, module);
                return;
            }

            foreach (var (name, alias) in imports)
            {
                if (name == Wildcard)
                {
                    foreach (var (k, v) in map)
                    {
                        if (scope.ContainsKey(k))
                            throw new Panic($"'{k}' already exists in the global scope");

                        scope.Set(k, v);
                    }

                    continue;
                }

                if (!map.TryGetValue(name, out var value))
                    throw new Panic($"module '{moduleName}' has no member '{name}'");

                if (scope.ContainsKey(alias))
                    throw new Panic($"'{alias}' already exists in the global scope");

                scope.Set(alias, value);
            }

            return;
        }

        var objectName = moduleAlias ?? moduleObjectName;

        if (scope.ContainsKey(objectName))
            throw new Panic($"'{objectName}' already exists in the global scope");

        scope.Set(objectName, new Obj(UnType.Create(objectName))
        {
            Members = new(map)
        });
    }

    private static Map BuildNativeMap(string name)
    {
        var type = natives.GetValueOrDefault(name) ?? throw new Panic($"native module '{name}' not found");

        foreach (var _class in type.GetCustomAttribute<NativeModuleAttribute>()!.Types)
        {
            var className = _class.GetCustomAttribute<NativeTypeAttribute>()!.Name!;
            if (natives.TryGetValue(className, out var t))
                originalClasses.Add(className, BuildNativeMap(className));
            else
                originalClasses.Add(className, CreateMethod(_class));
        }

        return CreateMethod(type);
    }

    private static Map Load(string fullPath)
    {
        Map map = [];

        var filePath = fullPath.EndsWith(".un") ? fullPath : fullPath + ".un";

        if (File.Exists(filePath))
        {
            LoadFile(filePath);
            return map;
        }
  
        if (Directory.Exists(fullPath))
        {
            var mod = Path.Combine(fullPath, "mod.un");

            if (File.Exists(mod))
            {
                LoadFile(mod);
                return map;
            }

            foreach (var file in Directory.GetFiles(fullPath, "*.un"))
                LoadFile(file);

            return map;
        }

        throw new Panic($"module '{fullPath}' not found");

        void LoadFile(string file)
        {
            var inner = new Scope(GetGlobalScope());

            Runner.Load(file, inner).Run();

            var symbols = inner.GetSymbolTable();
            var slots = inner.GetSlots();

            foreach (var (key, index) in symbols)
            {
                if (slots[index] is Obj obj)
                    map[key] = obj;
            }
        }
    }

    private static Map CreateMethod(Type type)
    {
        var map = new Map();

        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
        {
            var native = method.GetCustomAttribute<NativeAttribute>();
            if (native is null)
                continue;

            var fn = new NFn
            {
                Name = native.Name ?? method.Name,
                Func = args =>
                {
                    var parameters = method.GetParameters();
                    var values = new object?[parameters.Length];

                    for (int i = 0; i < parameters.Length; i++)
                        values[i] = args[parameters[i].Name!];

                    return (Obj)method.Invoke(null, values)!;
                }
            };

            foreach (var parameter in method.GetParameters())
            {
                if (parameter.GetCustomAttribute<SelfAttribute>() is not null)
                    continue;

                var info = parameter.GetCustomAttribute<ArgInfoAttribute>();

                fn.Args.Add(new Arg(parameter.Name!)
                {
                    IsEssential = info?.Essential ?? false,
                    IsOptional = info?.Optional ?? false,
                    IsPositional = info?.Positional ?? false,
                });
            }

            map.Add(fn.Name, fn);

        }

        return map;
    }

    public static bool IsNative(string name) => natives.ContainsKey(name);

    public static bool IsClass(string name) => classes.ContainsKey(name);

    public static Obj GetClass(string name)
    {
        if (classes.Get(name, out var obj))
            return obj;

        return new Err($"class '{name}' not found");
    }

    public static Obj GetClass(TObj type) => GetClass(type.Value);

    public static Obj GetClass(BaseType type)
    {
        if (type is UnType unType)        
            return GetClass(unType.Name);        
        else if (type is CollectionType colType)
            return GetClass(colType.Kind);

        return new Err($"type '{type}' is not a class");
    }

    public static bool TryGetClass(string name, out Obj obj)
    {
        if (classes.Get(name, out obj))
            return true;

        obj = null!;
        return false;
    }

    public static bool TryGetClass(BaseType type, out Obj obj)
    {
        if (type is UnType unType)
            return classes.Get(unType.Name, out obj);
        else if (type is CollectionType colType)
            return classes.Get(colType.Kind.Name, out obj);

        obj = null!;
        return false;
    }

    public static void SetClass(string name, Obj obj)
    {
        classes[name] = obj;
    }

    public static bool TryGetOriginalValue(string type, string name, out Obj? value)
    {
        if (originalClasses.TryGetValue(type, out var original))
            return original.TryGetValue(name, out value);

        value = null!;
        return false;
    }

    public static Obj GetGlobalVariable(string name) => scope.Get(name, out var value) ? value : new Err($"global variable '{name}' not found");

    public static void SetGlobalVariable(string name, Obj obj) => scope.Set(name, obj);

    public static bool TryGetGlobalVariable(string name, out Obj obj) => scope.Get(name, out obj);

    public static Scope GetGlobalScope() => scope;

    public static Map New(this Map map)
    {
        Map newMap = [];
        foreach (var (key, value) in map)
            if (value is Obj obj)
                newMap[key] = obj.Clone();
        return newMap;
    }
}