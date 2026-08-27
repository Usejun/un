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

    [Native(
        Name = "env",
        Description = "Reads an environment variable by name.",
        Example = "write(env(\"HOME\"))",
        ReturnType = "string",
        ArgumentTypes = new[] { "string" }
    )]
    public static Obj Env([ArgInfo(Essential = true)] Obj name)
    {
        if (!GetString(name, out var n, out var err))
            return err;

        var value = Environment.GetEnvironmentVariable(n);

        if (value == null)
            return new Err("environment variable not found");

        return Str.From(value);
    }

    [Native(
        Name = "setenv",
        Description = "Sets an environment variable.",
        Example = "setenv(\"MODE\", \"test\")",
        ReturnType = "none",
        ArgumentTypes = new[] { "string", "string" }
    )]
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

    [Native(
        Name = "unsetenv",
        Description = "Removes an environment variable.",
        Example = "unsetenv(\"MODE\")",
        ReturnType = "none",
        ArgumentTypes = new[] { "string" }
    )]
    public static Obj UnsetEnv([ArgInfo(Essential = true)] Obj name)
    {
        if (!GetString(name, out var n, out var err))
            return err;

        Environment.SetEnvironmentVariable(n, null);

        return Obj.None;
    }

    [Native(
        Name = "environ",
        Description = "Returns all environment variables as text entries.",
        Example = "values = environ()",
        ReturnType = "list"
    )]
    public static Obj Environ()
    {
        var vars = Environment.GetEnvironmentVariables();
        var list = new List<Obj>();

        foreach (System.Collections.DictionaryEntry entry in vars)
            list.Add(Str.From($"{entry.Key}={entry.Value}"));

        return new List([.. list]);
    }

    [Native(
        Name = "expand_vars",
        Description = "Expands environment-variable placeholders in text.",
        Example = "path = expand_vars(\"$HOME/data\")",
        ReturnType = "string",
        ArgumentTypes = new[] { "string" }
    )]
    public static Obj ExpandVars([ArgInfo(Essential = true)] Obj text)
    {
        if (!GetString(text, out var t, out var err))
            return err;

        return Str.From(Environment.ExpandEnvironmentVariables(t));
    }

    [Native(
        Name = "pid",
        Description = "Returns the current process identifier.",
        Example = "write(pid())",
        ReturnType = "integer"
    )]
    public static Obj Pid() => Int.From(Environment.ProcessId);

    [Native(
        Name = "name",
        Description = "Returns the current operating-system name.",
        Example = "write(name())",
        ReturnType = "string"
    )]
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

    [Native(
        Name = "arch",
        Description = "Returns the operating-system architecture.",
        Example = "write(arch())",
        ReturnType = "string"
    )]
    public static Obj Arch() => Str.From(System.Runtime.InteropServices.RuntimeInformation.OSArchitecture.ToString());

    [Native(
        Name = "version",
        Description = "Returns the operating-system version.",
        Example = "write(version())",
        ReturnType = "string"
    )]
    public static Obj Version() => Str.From(Environment.OSVersion.VersionString);

    [Native(
        Name = "cpuCount",
        Description = "Returns the available processor count.",
        Example = "write(cpuCount())",
        ReturnType = "integer"
    )]
    public static Obj CpuCount() => Int.From(Environment.ProcessorCount);

    [Native(
        Name = "is64bit",
        Description = "Checks whether the operating system is 64-bit.",
        Example = "write(is64bit())",
        ReturnType = "boolean"
    )]
    public static Obj Is64Bit() => Bool.From(Environment.Is64BitOperatingSystem);

    [Native(
        Name = "hostname",
        Description = "Returns the current machine host name.",
        Example = "write(hostname())",
        ReturnType = "string"
    )]
    public static Obj Hostname() => Str.From(Environment.MachineName);

    [Native(
        Name = "username",
        Description = "Returns the current operating-system user name.",
        Example = "write(username())",
        ReturnType = "string"
    )]
    public static Obj Username() => Str.From(Environment.UserName);

    [Native(
        Name = "args",
        Description = "Returns the process command-line arguments.",
        Example = "values = args()",
        ReturnType = "list"
    )]
    public static List Args() => new([.. Environment.GetCommandLineArgs().Select(Str.From)]);

    [Native(
        Name = "sep",
        Description = "Returns the directory separator character.",
        Example = "write(sep())",
        ReturnType = "string"
    )]
    public static Str Sep() => Str.From(Path.DirectorySeparatorChar.ToString());

    [Native(
        Name = "pathsep",
        Description = "Returns the path-list separator character.",
        Example = "write(pathsep())",
        ReturnType = "string"
    )]
    public static Str PathSep() => Str.From(Path.PathSeparator.ToString());

    [Native(
        Name = "linesep",
        Description = "Returns the environment line separator.",
        Example = "write(linesep())",
        ReturnType = "string"
    )]
    public static Str LineSep() => Str.From(Environment.NewLine);

    [Native(
        Name = "tickCount",
        Description = "Returns elapsed system ticks in milliseconds.",
        Example = "write(tickCount())",
        ReturnType = "integer"
    )]
    public static Int TickCount() => Int.From(Environment.TickCount64);

    [Native(
        Name = "exit",
        Description = "Stops the current process with an optional exit code.",
        Example = "exit(0)",
        ReturnType = "list",
        ArgumentTypes = new[] { "integer" }
    )]
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

    [Native(
        Name = "exec",
        Description = "Runs a shell command and returns its standard output.",
        Example = "output = exec(\"echo hello\")",
        ReturnType = "string",
        ArgumentTypes = new[] { "string" }
    )]
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
