using Un.Object;
using Un.Object.Collections;
using Un.Object.Primitive;
using Un.Reflection;

namespace Un.Native;

[NativeModule("fs")]
public static class Fs
{
    static bool GetPath(Obj obj, out string path, out Obj err)
    {
        if (!obj.As<Str>(out var value))
        {
            path = "";
            err = new Err("expected path is string");
            return false;
        }

        path = value.Value;
        err = Obj.None;
        return true;
    }

    [Native(Name = "exists")]
    public static Obj Exists([ArgInfo(Essential = true)] Obj path)
    {
        if (!GetPath(path, out var p, out var err))
            return err;

        return Bool.From(File.Exists(p) || Directory.Exists(p));
    }

    [Native(Name = "file")]
    public static Obj FileExists([ArgInfo(Essential = true)] Obj path)
    {
        if (!GetPath(path, out var p, out var err))
            return err;

        return Bool.From(File.Exists(p));
    }

    [Native(Name = "dir")]
    public static Obj DirectoryExists([ArgInfo(Essential = true)] Obj path)
    {
        if (!GetPath(path, out var p, out var err))
            return err;

        return Bool.From(Directory.Exists(p));
    }

    [Native(Name = "read")]
    public static Obj Read([ArgInfo(Essential = true)] Obj path)
    {
        if (!GetPath(path, out var p, out var err))
            return err;

        if (!File.Exists(p))
            return new Err("file not found");

        return Str.From(System.IO.File.ReadAllText(p));
    }

    [Native(Name = "write")]
    public static Obj Write(
        [ArgInfo(Essential = true)] Obj path,
        [ArgInfo(Essential = true)] Obj text)
    {
        if (!GetPath(path, out var p, out var err))
            return err;

        if (!text.As<Str>(out var value))
            return new Err("expected text is string");

        File.WriteAllText(p, value.Value);

        return Obj.None;
    }

    [Native(Name = "append")]
    public static Obj Append(
        [ArgInfo(Essential = true)] Obj path,
        [ArgInfo(Essential = true)] Obj text)
    {
        if (!GetPath(path, out var p, out var err))
            return err;

        if (!text.As<Str>(out var value))
            return new Err("expected text is string");

        File.AppendAllText(p, value.Value);

        return Obj.None;
    }

    [Native(Name = "delete")]
    public static Obj Delete([ArgInfo(Essential = true)] Obj path)
    {
        if (!GetPath(path, out var p, out var err))
            return err;

        if (File.Exists(p))
            File.Delete(p);
        else if (Directory.Exists(p))
            Directory.Delete(p, true);

        return Obj.None;
    }

    [Native(Name = "copy")]
    public static Obj Copy(
        [ArgInfo(Essential = true)] Obj source,
        [ArgInfo(Essential = true)] Obj destination)
    {
        if (!GetPath(source, out var src, out var err))
            return err;

        if (!GetPath(destination, out var dst, out err))
            return err;

        File.Copy(src, dst, true);

        return Obj.None;
    }

    [Native(Name = "move")]
    public static Obj Move(
        [ArgInfo(Essential = true)] Obj source,
        [ArgInfo(Essential = true)] Obj destination)
    {
        if (!GetPath(source, out var src, out var err))
            return err;

        if (!GetPath(destination, out var dst, out err))
            return err;

        File.Move(src, dst, true);

        return Obj.None;
    }

    [Native(Name = "rename")]
    public static Obj Rename(
        [ArgInfo(Essential = true)] Obj path,
        [ArgInfo(Essential = true)] Obj name)
    {
        if (!GetPath(path, out var src, out var err))
            return err;

        if (!name.As<Str>(out var value))
            return new Err("expected name is string");

        string parent = Path.GetDirectoryName(src)!;
        string dst = Path.Combine(parent, value.Value);

        File.Move(src, dst, true);

        return Obj.None;
    }

    [Native(Name = "touch")]
    public static Obj Touch([ArgInfo(Essential = true)] Obj path)
    {
        if (!GetPath(path, out var p, out var err))
            return err;

        if (!File.Exists(p))
            File.Create(p).Dispose();

        return Obj.None;
    }

    [Native(Name = "mkdir")]
    public static Obj Mkdir([ArgInfo(Essential = true)] Obj path)
    {
        if (!GetPath(path, out var p, out var err))
            return err;

        Directory.CreateDirectory(p);

        return Obj.None;
    }

    [Native(Name = "rmdir")]
    public static Obj Rmdir(
        [ArgInfo(Essential = true)] Obj path,
        [ArgInfo(Optional = true)] Obj recursive = null!)
    {
        if (!GetPath(path, out var p, out var err))
            return err;

        bool value = true;

        if (recursive != null)
        {
            if (!recursive.As<Bool>(out var b))
                return new Err("expected recursive is bool");

            value = b.Value;
        }

        Directory.Delete(p, value);

        return Obj.None;
    }

    [Native(Name = "list")]
    public static Obj List([ArgInfo(Essential = true)] Obj path)
    {
        if (!GetPath(path, out var p, out var err))
            return err;

        if (!Directory.Exists(p))
            return new Err("directory not found");

        return new List([..Directory.EnumerateFileSystemEntries(p).Select(Str.From)]);
    }

    [Native(Name = "files")]
    public static Obj Files([ArgInfo(Essential = true)] Obj path)
    {
        if (!GetPath(path, out var p, out var err))
            return err;

        if (!Directory.Exists(p))
            return new Err("directory not found");

        return new List([..Directory.EnumerateFiles(p).Select(Str.From)]);
    }

    [Native(Name = "dirs")]
    public static Obj Dirs([ArgInfo(Essential = true)] Obj path)
    {
        if (!GetPath(path, out var p, out var err))
            return err;

        if (!Directory.Exists(p))
            return new Err("directory not found");

        return new List([..Directory.EnumerateDirectories(p).Select(Str.From)]);
    }

    [Native(Name = "walk")]
    public static Obj Walk([ArgInfo(Essential = true)] Obj path)
    {
        if (!GetPath(path, out var p, out var err))
            return err;

        if (!Directory.Exists(p))
            return new Err("directory not found");

        return new List([..Directory.EnumerateFileSystemEntries(p, "*", SearchOption.AllDirectories).Select(Str.From)]);
    }

    [Native(Name = "size")]
    public static Obj Size([ArgInfo(Essential = true)] Obj path)
    {
        if (!GetPath(path, out var p, out var err))
            return err;

        if (!File.Exists(p))
            return new Err("file not found");

        return Int.From(new FileInfo(p).Length);
    }

    [Native(Name = "created")]
    public static Obj Created([ArgInfo(Essential = true)] Obj path)
    {
        if (!GetPath(path, out var p, out var err))
            return err;

        DateTime time = File.Exists(p) ? File.GetCreationTimeUtc(p) : Directory.GetCreationTimeUtc(p);

        return Str.From(time.ToString("O"));
    }

    [Native(Name = "modified")]
    public static Obj Modified([ArgInfo(Essential = true)] Obj path)
    {
        if (!GetPath(path, out var p, out var err))
            return err;

        DateTime time = File.Exists(p) ? File.GetLastWriteTimeUtc(p) : Directory.GetLastWriteTimeUtc(p);

        return Str.From(time.ToString("O"));
    }

    [Native(Name = "accessed")]
    public static Obj Accessed([ArgInfo(Essential = true)] Obj path)
    {
        if (!GetPath(path, out var p, out var err))
            return err;

        DateTime time = File.Exists(p)
            ? File.GetLastAccessTimeUtc(p)
            : Directory.GetLastAccessTimeUtc(p);

        return Str.From(time.ToString("O"));
    }

    [Native(Name = "readonly")]
    public static Obj Readonly([ArgInfo(Essential = true)] Obj path)
    {
        if (!GetPath(path, out var p, out var err))
            return err;

        if (!File.Exists(p))
            return new Err("file not found");

        return Bool.From(File.GetAttributes(p).HasFlag(FileAttributes.ReadOnly));
    }

    [Native(Name = "hidden")]
    public static Obj Hidden([ArgInfo(Essential = true)] Obj path)
    {
        if (!GetPath(path, out var p, out var err))
            return err;

        if (!File.Exists(p) && !Directory.Exists(p))
            return new Err("path not found");

        return Bool.From(File.GetAttributes(p).HasFlag(FileAttributes.Hidden));
    }

    [Native(Name = "join")]
    public static Obj Join([ArgInfo(Positional = true)] Obj paths)
    {
        if (!paths.As<Tup>(out var tuple))
            return new Err("expected tuple");

        var values = new string[tuple.Count];

        for (int i = 0; i < tuple.Count; i++)
        {
            if (!tuple[i].As<Str>(out var value))
                return new Err("expected path is string");

            values[i] = value.Value;
        }

        return Str.From(Path.Combine(values));
    }

    [Native(Name = "parent")]
    public static Obj Parent([ArgInfo(Essential = true)] Obj path)
    {
        if (!GetPath(path, out var p, out var err))
            return err;

        return Str.From(Path.GetDirectoryName(p) ?? "");
    }

    [Native(Name = "name")]
    public static Obj Name([ArgInfo(Essential = true)] Obj path)
    {
        if (!GetPath(path, out var p, out var err))
            return err;

        return Str.From(Path.GetFileName(p));
    }

    [Native(Name = "stem")]
    public static Obj Stem([ArgInfo(Essential = true)] Obj path)
    {
        if (!GetPath(path, out var p, out var err))
            return err;

        return Str.From(Path.GetFileNameWithoutExtension(p));
    }

    [Native(Name = "ext")]
    public static Obj Extension([ArgInfo(Essential = true)] Obj path)
    {
        if (!GetPath(path, out var p, out var err))
            return err;

        return Str.From(Path.GetExtension(p));
    }

    [Native(Name = "absolute")]
    public static Obj Absolute([ArgInfo(Essential = true)] Obj path)
    {
        if (!GetPath(path, out var p, out var err))
            return err;

        return Str.From(Path.GetFullPath(p));
    }

    [Native(Name = "relative")]
    public static Obj Relative(
        [ArgInfo(Essential = true)] Obj from,
        [ArgInfo(Essential = true)] Obj to)
    {
        if (!GetPath(from, out var f, out var err))
            return err;

        if (!GetPath(to, out var t, out err))
            return err;

        return Str.From(Path.GetRelativePath(f, t));
    }

    [Native(Name = "normalize")]
    public static Obj Normalize([ArgInfo(Essential = true)] Obj path)
    {
        if (!GetPath(path, out var p, out var err))
            return err;

        return Str.From(Path.GetFullPath(Path.TrimEndingDirectorySeparator(p)));
    }

    [Native(Name = "cwd")]
    public static Obj Cwd() => Str.From(Environment.CurrentDirectory);

    [Native(Name = "chdir")]
    public static Obj Chdir([ArgInfo(Essential = true)] Obj path)
    {
        if (!GetPath(path, out var p, out var err))
            return err;

        if (!Directory.Exists(p))
            return new Err("directory not found");

        Environment.CurrentDirectory = p;
        return Obj.None;
    }

    [Native(Name = "temp")]
    public static Str Temp() => Str.From(Path.GetTempPath());

    [Native(Name = "home")]
    public static Obj Home() => Str.From(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

    [Native(Name = "exe")]
    public static Str Exe() => Str.From(Environment.ProcessPath ?? "");

    [Native(Name = "app")]
    public static Str App() => Str.From(AppContext.BaseDirectory);
}