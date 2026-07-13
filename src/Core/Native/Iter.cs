using Un.Object;
using Un.Object.Collections;
using Un.Object.Iter;
using Un.Object.Primitive;
using Un.Reflection;

namespace Un.Native;

[NativeModule("iter", typeof(Object.Iter.Range), typeof(Counter), typeof(Reverse))]
public static class Iter
{
    [Native(Name = "iter")]
    public static Obj Array(
        [ArgInfo(Essential = true)] Obj value,
        [ArgInfo(Positional = true)] Obj size = null!)
    {
        size ??= new Tup([Int.From(1)]);

        if (size.ToList().As<List>(out var sizeList))
            return new Err("expected 'size' argument to be a 'list'");

        var sizes = new List<int>();

        foreach (var item in sizeList.Value)
        {
            if (!item.ToInt().As<Int>(out var intItem))
                return new Err("expected all items in 'size' list to be integers");
            sizes.Add((int)intItem.Value);
        }

        return Create([.. sizes]);

        List Create(int[] lengths)
        {
            List list = [];

            for (int i = 0; i < lengths[0]; i++)
                List.Append(list, lengths.Length == 1 ? value.Clone() : Create([.. lengths[1..]]));

            return list;
        }
    }

    [Native(Name = "range")]
    public static Obj Range(
        [ArgInfo(Essential = true)] Obj start,
        [ArgInfo(Optional = true)] Obj end = null!,
        [ArgInfo(Optional = true)] Obj step = null!)
    {
        end ??= Int.From(0);
        step ??= Int.From(1);

        if (!start.ToInt().As<Int>(out var intStart))
            return new Err("expected 'start' argument to be an integer");
        if (!end.ToInt().As<Int>(out var intEnd))
            return new Err("expected 'end' argument to be an integer");
        if (!step.ToInt().As<Int>(out var intStep))
            return new Err("expected 'step' argument to be an integer");

        if (intStep.Value == 0)
            return new Err("step argument must not be zero");

        if (intStart.Value > intEnd.Value && intStep.Value > 0)
            (intStart, intEnd) = (intEnd, intStart);

        return new Object.Iter.Range(intStart.Value, intEnd.Value, intStep.Value);
    }

    [Native(Name = "counter")]
    public static Obj Counter([ArgInfo(Optional = true)] Obj start = null!)
    {
        start ??= Int.From(0);

        if (!start.ToInt().As<Int>(out var intStart))
            return new Err("expected 'start' argument to be an integer");
        return new Counter(intStart.Value);
    }

    [Native(Name = "reverse")]
    public static Obj Reverse([ArgInfo(Essential = true)] Obj obj)
    {
        if (!obj.Iter().As<Iters>(out var iter))
            return new Err("expected 'array' argument to be of type 'list'");
        return new Reverse(iter.Value);
    }

    [Native(Name = "zip")]
    public static Obj Zip(
        [ArgInfo(Positional = true)] Obj iterables)
    {
        if (!iterables.As<Iters>(out var arrays))
            return new Err("expected 'arrays' argument to be of type 'iter'");

        if (arrays.Len().ToInt().As<Int>(out var intLen) && intLen.Value == 0)
            return new Err("expected 'arrays' argument to have a non-zero length");

        int length = int.MaxValue;

        foreach (var i in arrays.Value)
        {
            if (!i.Len().As<Int>(out var arrayLength))
                return new Err("expected all arrays to have a valid length");
            if (arrayLength.Value < length)
                length = (int)arrayLength.Value;
        }


        List list = [];

        for (int i = 0; i < length; i++)
        {
            List buf = [];
            foreach (var array in arrays.Value)
                buf.Append(array.GetItem(Int.From(i)));

            list.Append(new Tup([.. buf], new string[buf.Count]));
        }

        return list;
    }

    [Native(Name = "enumerate")]
    public static Obj Enumerate([ArgInfo(Essential = true)] Obj iterable)
    {
        if (!iterable.Iter().As<Iters>(out var iter))
            return new Err("expected 'array' argument to be of type 'iter'");

        return new Iters(iter.Value.Select((x, i) => new Tup([Int.From(i), x], ["index", "value"])));
    }

    [Native(Name = "sum")]
    public static Obj Sum([ArgInfo(Positional = true)] Obj iterable)
    {
        if (!iterable.ToTuple().As<Tup>(out var tuple))
            return new Err("expected 'value' argument to be a tuple");

        Obj values = tuple switch
        {
            { Count: 0 } => new Err("expected more than one argument"),
            { Count: 1 } when tuple[0].As<List>(out var l) => l.ToTuple(),
            { Count: 1 } when tuple[0].As<Tup>(out var t) => t,
            { Count: 1 } when tuple[0].As<Object.Iter.Range>(out var r) => r.ToTuple(),
            { Count: 1 } when tuple[0].As<Iters>(out var it) && it.As<Tup>(out var itTuple) => itTuple,
            { Count: 1 } when tuple[0].As<Iters>(out _) => new Err("invalid argument type of 'value'"),
            { Count: 1 } => tuple[0],
            _ => tuple
        };

        if (values is Err)
            return values;
        if (!values.ToTuple().As<Tup>(out var valuesTuple))
            return new Err("expected 'value' argument to be a tuple");

        Obj total = valuesTuple[0];

        for (int i = 1; i < valuesTuple.Count; i++)
        {
            var sum = total.Add(valuesTuple[i]);

            if (sum is Err)
                return sum;
        }

        return total;
    }

    [Native(Name = "max")]
    public static Obj Max([ArgInfo(Positional = true)] Obj iterable)
    {
        if (!iterable.ToTuple().As<Tup>(out var tuple))
            return new Err("expected 'value' argument to be a tuple");

        Obj values = tuple switch
        {
            { Count: 0 } => new Err("expected more than one argument"),
            { Count: 1 } when tuple[0].As<List>(out var l) => l.ToTuple(),
            { Count: 1 } when tuple[0].As<Tup>(out var t) => t,
            { Count: 1 } when tuple[0].As<Object.Iter.Range>(out var r) => r.ToTuple(),
            { Count: 1 } when tuple[0].As<Iters>(out var it) && it.As<Tup>(out var itTuple) => itTuple,
            { Count: 1 } when tuple[0].As<Iters>(out _) => new Err("invalid argument type of 'value'"),
            { Count: 1 } => tuple[0],
            _ => tuple
        };

        if (values is Err)
            return values;
        if (!values.ToTuple().As<Tup>(out var valuesTuple))
            return new Err("expected 'value' argument to be a tuple");

        Obj max = valuesTuple[0];
        for (int i = 1; i < valuesTuple.Count; i++)
        {
            var lt = max.Lt(valuesTuple[i]);

            if (!lt.As<Bool>(out var isLess))
                return new Err($"{max} < {valuesTuple[i]} is not a boolean");

            if (isLess.Value)
                max = valuesTuple[i];
        }

        return max;
    }

    [Native(Name = "min")]
    public static Obj Min([ArgInfo(Positional = true)] Obj iterable)
    {
        if (!iterable.ToTuple().As<Tup>(out var tuple))
            return new Err("expected 'value' argument to be a tuple");

        Obj values = tuple switch
        {
            { Count: 0 } => new Err("expected more than one argument"),
            { Count: 1 } when tuple[0].As<List>(out var l) => l.ToTuple(),
            { Count: 1 } when tuple[0].As<Tup>(out var t) => t,
            { Count: 1 } when tuple[0].As<Object.Iter.Range>(out var r) => r.ToTuple(),
            { Count: 1 } when tuple[0].As<Iters>(out var it) && it.As<Tup>(out var itTuple) => itTuple,
            { Count: 1 } when tuple[0].As<Iters>(out _) => new Err("invalid argument type of 'value'"),
            { Count: 1 } => tuple[0],
            _ => tuple
        };

        if (values is Err)
            return values;
        if (!values.ToTuple().As<Tup>(out var valuesTuple))
            return new Err("expected 'value' argument to be a tuple");

        Obj min = valuesTuple[0];
        for (int i = 1; i < valuesTuple.Count; i++)
        {
            var gt = min.Gt(valuesTuple[i]);

            if (!gt.As<Bool>(out var isGreater))
                return new Err($"{min} > {valuesTuple[i]} is not a boolean");

            if (isGreater.Value)
                min = valuesTuple[i];
        }

        return min;
    }
}
