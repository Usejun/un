using System.Diagnostics;
using Un.Object;
using Un.Object.Collections;
using Un.Object.Primitive;
using Un.Reflection;

namespace Un.Native;

[NativeModule("os")]
public static class Os
{
    static bool GetString(Obj obj, out string value, out Obj err)
    {
        if (!obj.As<Str>(out var str))
        {
            value = "";
            err = new Err("expected value is string");
            return false;
        }

        value = str.Value;
        err = Obj.None;
        return true;
    }

    [Native(Name = "env")]
    public static Obj Env([ArgInfo(Essential = true)] Obj name)
    {
        if (!GetString(name, out var n, out var err))
            return err;

        var value = Environment.GetEnvironmentVariable(n);

        if (value == null)
            return new Err("environment variable not found");

        return Str.From(value);
    }

    [Native(Name = "setenv")]
    public static Obj SetEnv(
        [ArgInfo(Essential = true)] Obj name,
        [ArgInfo(Essential = true)] Obj value)
    {
        if (!GetString(name, out var n, out var err))
            return err;

        if (!GetString(value, out var v, out err))
            return err;

        Environment.SetEnvironmentVariable(n, v);

        return Obj.None;
    }

    [Native(Name = "unsetenv")]
    public static Obj UnsetEnv([ArgInfo(Essential = true)] Obj name)
    {
        if (!GetString(name, out var n, out var err))
            return err;

        Environment.SetEnvironmentVariable(n, null);

        return Obj.None;
    }

    [Native(Name = "environ")]
    public static Obj Environ()
    {
        var vars = Environment.GetEnvironmentVariables();
        var list = new List<Obj>();

        foreach (System.Collections.DictionaryEntry entry in vars)
            list.Add(Str.From($"{entry.Key}={entry.Value}"));

        return new List([.. list]);
    }

    [Native(Name = "expand_vars")]
    public static Obj ExpandVars([ArgInfo(Essential = true)] Obj text)
    {
        if (!GetString(text, out var t, out var err))
            return err;

        return Str.From(Environment.ExpandEnvironmentVariables(t));
    }

    [Native(Name = "pid")]
    public static Obj Pid() => Int.From(Environment.ProcessId);

    [Native(Name = "name")]
    public static Obj Name()
    {
        if (OperatingSystem.IsWindows())
            return Str.From("windows");

        if (OperatingSystem.IsLinux())
            return Str.From("linux");

        if (OperatingSystem.IsMacOS())
            return Str.From("macos");

        return Str.From("unknown");
    }

    [Native(Name = "arch")]
    public static Obj Arch() => Str.From(System.Runtime.InteropServices.RuntimeInformation.OSArchitecture.ToString());

    [Native(Name = "version")]
    public static Obj Version() => Str.From(Environment.OSVersion.VersionString);

    [Native(Name = "cpuCount")]
    public static Obj CpuCount() => Int.From(Environment.ProcessorCount);

    [Native(Name = "is64bit")]
    public static Obj Is64Bit() => Bool.From(Environment.Is64BitOperatingSystem);

    [Native(Name = "hostname")]
    public static Obj Hostname() => Str.From(Environment.MachineName);

    [Native(Name = "username")]
    public static Obj Username() => Str.From(Environment.UserName);

    [Native(Name = "args")]
    public static List Args() => new([..Environment.GetCommandLineArgs().Select(Str.From)]);

    [Native(Name = "sep")]
    public static Str Sep() => Str.From(Path.DirectorySeparatorChar.ToString());

    [Native(Name = "pathsep")]
    public static Str PathSep() => Str.From(Path.PathSeparator.ToString());

    [Native(Name = "linesep")]
    public static Str LineSep() => Str.From(Environment.NewLine);

    [Native(Name = "tickCount")]
    public static Int TickCount() => Int.From(Environment.TickCount64);

    [Native(Name = "exit")]
    public static Obj Exit([ArgInfo(Optional = true)] Obj code = null!)
    {
        int value = 0;

        if (code != null)
        {
            if (!code.As<Int>(out var i))
                return new Err("expected code is int");

            value = (int)i.Value;
        }

        Environment.Exit(value);

        return Obj.None;
    }

    [Native(Name = "exec")]
    public static Obj Exec([ArgInfo(Essential = true)] Obj command)
    {
        if (!GetString(command, out var cmd, out var err))
            return err;

        var isWindows = OperatingSystem.IsWindows();

        var psi = new ProcessStartInfo
        {
            FileName = isWindows ? "cmd.exe" : "/bin/sh",
            Arguments = isWindows ? $"/c {cmd}" : $"-c \"{cmd}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        try
        {
            using var process = Process.Start(psi);

            if (process == null)
                return new Err("failed to start process");

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return Str.From(output);
        }
        catch (Exception e)
        {
            return new Err(e.Message);
        }
    }
}