using System.Text.RegularExpressions;
using Un.Object;
using Un.Object.Collections;
using Un.Object.Primitive;
using Un.Reflection;

namespace Un.Native;

[NativeModule("re")]
public static class Re
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

    static bool TryCompile(string pattern, out Regex regex, out Obj err)
    {
        try
        {
            regex = new Regex(pattern);
            err = Obj.None;
            return true;
        }
        catch (ArgumentException e)
        {
            regex = null!;
            err = new Err($"invalid pattern: {e.Message}");
            return false;
        }
    }

    [Native(Name = "test")]
    public static Obj Test(
        [ArgInfo(Essential = true)] Obj pattern,
        [ArgInfo(Essential = true)] Obj text)
    {
        if (!GetString(pattern, out var p, out var err))
            return err;

        if (!GetString(text, out var t, out err))
            return err;

        if (!TryCompile(p, out var regex, out err))
            return err;

        return Bool.From(regex.IsMatch(t));
    }

    [Native(Name = "match")]
    public static Obj Match(
        [ArgInfo(Essential = true)] Obj pattern,
        [ArgInfo(Essential = true)] Obj text)
    {
        if (!GetString(pattern, out var p, out var err))
            return err;

        if (!GetString(text, out var t, out err))
            return err;

        if (!TryCompile(p, out var regex, out err))
            return err;

        var m = regex.Match(t);

        if (!m.Success || m.Index != 0)
            return Obj.None;

        return Str.From(m.Value);
    }

    [Native(Name = "search")]
    public static Obj Search(
        [ArgInfo(Essential = true)] Obj pattern,
        [ArgInfo(Essential = true)] Obj text)
    {
        if (!GetString(pattern, out var p, out var err))
            return err;

        if (!GetString(text, out var t, out err))
            return err;

        if (!TryCompile(p, out var regex, out err))
            return err;

        var m = regex.Match(t);

        return m.Success ? Str.From(m.Value) : Obj.None;
    }

    [Native(Name = "find_all")]
    public static Obj FindAll(
        [ArgInfo(Essential = true)] Obj pattern,
        [ArgInfo(Essential = true)] Obj text)
    {
        if (!GetString(pattern, out var p, out var err))
            return err;

        if (!GetString(text, out var t, out err))
            return err;

        if (!TryCompile(p, out var regex, out err))
            return err;

        return new List([
            ..regex.Matches(t).Select(m => (Obj)Str.From(m.Value))
        ]);
    }

    [Native(Name = "groups")]
    public static Obj Groups(
        [ArgInfo(Essential = true)] Obj pattern,
        [ArgInfo(Essential = true)] Obj text)
    {
        if (!GetString(pattern, out var p, out var err))
            return err;

        if (!GetString(text, out var t, out err))
            return err;

        if (!TryCompile(p, out var regex, out err))
            return err;

        var m = regex.Match(t);

        if (!m.Success)
            return Obj.None;

        var list = new System.Collections.Generic.List<Obj>();

        for (int i = 1; i < m.Groups.Count; i++)
        {
            var group = m.Groups[i];
            list.Add(group.Success ? Str.From(group.Value) : Obj.None);
        }

        return new List([.. list]);
    }

    [Native(Name = "replace")]
    public static Obj Replace(
        [ArgInfo(Essential = true)] Obj pattern,
        [ArgInfo(Essential = true)] Obj text,
        [ArgInfo(Essential = true)] Obj replacement)
    {
        if (!GetString(pattern, out var p, out var err))
            return err;

        if (!GetString(text, out var t, out err))
            return err;

        if (!GetString(replacement, out var r, out err))
            return err;

        if (!TryCompile(p, out var regex, out err))
            return err;

        return Str.From(regex.Replace(t, r, 1));
    }

    [Native(Name = "replace_all")]
    public static Obj ReplaceAll(
        [ArgInfo(Essential = true)] Obj pattern,
        [ArgInfo(Essential = true)] Obj text,
        [ArgInfo(Essential = true)] Obj replacement)
    {
        if (!GetString(pattern, out var p, out var err))
            return err;

        if (!GetString(text, out var t, out err))
            return err;

        if (!GetString(replacement, out var r, out err))
            return err;

        if (!TryCompile(p, out var regex, out err))
            return err;

        return Str.From(regex.Replace(t, r));
    }

    [Native(Name = "split")]
    public static Obj Split(
        [ArgInfo(Essential = true)] Obj pattern,
        [ArgInfo(Essential = true)] Obj text)
    {
        if (!GetString(pattern, out var p, out var err))
            return err;

        if (!GetString(text, out var t, out err))
            return err;

        if (!TryCompile(p, out var regex, out err))
            return err;

        return new List([
            ..regex.Split(t).Select(Str.From)
        ]);
    }

    [Native(Name = "escape")]
    public static Obj Escape([ArgInfo(Essential = true)] Obj text)
    {
        if (!GetString(text, out var t, out var err))
            return err;

        return Str.From(Regex.Escape(t));
    }
}