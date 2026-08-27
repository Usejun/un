using System.Collections;
using Un.Object.Function;
using Un.Object.Primitive;
using Un.Object.Iter;
using Un.Object.Type;
using Un.Reflection;

namespace Un.Object.Collections;

[BuiltinType("list")]
public class List(Obj[] value) : Ref<Obj[]>(value, UnType.List), IEnumerable<Obj>
{
    public struct Enumerator(List list) : IEnumerator<Obj>
    {
        private readonly List list = list;
        private int index = -1;

        public readonly Obj Current => list[index];

        readonly object IEnumerator.Current => list[index];

        public bool MoveNext()
        {
            index++;
            return index < list.Count;
        }

        public void Reset()
        {
            index = -1;
        }

        public void Dispose()
        {

        }
    }

    public List() : this([]) { }

    public Obj this[int index]
    {
        get => Value[index];
        set => Value[index] = value;
    }

    public int Count { get; private set; } = value.Length;

    public bool IsFull => Count == Value.Length;

    public override Obj Init(Tup args) => args.ToList();

    public override Bool Eq(Obj other)
    {
        if (other is not List list)
            return Bool.False;

        for (int i = 0; i < Count; i++)
            if (Value[i].NEq(list[i]).As<Bool>(out var neq) && neq.Value)
                return Bool.False;

        return Bool.True;
    }

    public override Obj GetItem(Obj key) => key switch
    {
        Int i => OutOfRange(this, (int)i.Value) ? OutOfRange(this, (int)(i.Value + Count)) ? new Err("list index out of range") : this[(int)(i.Value + Count)] : this[(int)i.Value],
        _ => new Err("invalid index type")
    };

    public override Obj SetItem(Obj key, Obj value)
    {
        if (key is not Int i)
            return new Err("invalid index type");

        if (!OutOfRange(this, (int)i.Value))
            return this[(int)i.Value] = value;
            
        if (!OutOfRange(this, (int)(i.Value + Count)))
            return this[(int)(i.Value + Count)] = value;

        return new Err("list index out of range");
    }

    public override Obj In(Obj obj) => obj switch
    {
        List list => Bool.From(Overlap(this, list)),
        Tup tup => Bool.From(Overlap(this, tup.ToList())),

        _ => new Err($"cannot check if '{obj.Type}' is in '{Type}'"),
    };

    public override Int Len() => Int.From(Count);

    public override Bool ToBool() => Bool.From(Count != 0);

    public override List ToList()
    {
        var newList = new List(new Obj[Count]);
        for (int i = 0; i < Count; i++)
            newList[i] = this[i].Copy();
        return newList;
    }

    public override Tup ToTuple() => new(Value[..Count], new string[Count]);

    public override Iters Iter() => new(this);

    public override List Copy() => this;

    public override List Clone()
    {
        var newList = new List(new Obj[Count]);
        for (int i = 0; i < Count; i++)
            newList[i] = this[i].Clone();
        return newList;
    }

    public override Str ToStr() => Str.From($"[{string.Join(", ", Value[..Count].Select(Format))}]");

    private string Format(Obj value) => value is Str s ? $"'{s.Value}'" : Str.To(value).Value;

    public override Spreads Spread() => new(Value[..Count]);

    private static bool OutOfRange(List self, int index) => index < 0 || index >= self.Count;

    private static bool Overlap(List self, List list)
    {
        foreach (var item in list)
        {
            if (!self.Value.Contains(item))
                return false;
        }
        return true;
    }

    [Native(Name = "add")]
    public static void Append([Self] List self, [ArgInfo(Essential = true)] Obj value)
    {
        if (self.IsFull) Resize(self);
        self[self.Count] = value;
        self.Count++;
    }

    [Native(Name = "extend")]
    public static void Extend([Self] List self, [ArgInfo(Essential = true)] Obj value)
    {
        if (value.Iter().As<Iters>(out var iters))
        {
            foreach (var v in iters.Value)
                Append(self, v);
        }
        else           
        { 
            Append(self, value); 
        }
    }

    [Native(Name = "insert")]
    public static Obj Insert(
        [Self] List self,
        [ArgInfo(Essential = true)] Obj obj,
        [ArgInfo(Essential = true)] Obj index)
    {
        if (!index.As<Int>(out var indexValue))
            return new Err("invalid arguments: index");

        if (self.Count == 0)
        {
            Append(self, obj);
            return self;
        }

        if (self.IsFull)
            Resize(self);

        for (int i = self.Count - 1; i >= indexValue.Value; i--)
            self[i + 1] = self[i];
        self[(int)indexValue.Value] = obj.Copy();
        self.Count++;

        return self;
    }

    [Native(Name = "extend_insert")]
    public static List ExtendInsert(
        [Self] List self,
        [ArgInfo(Essential = true)] Obj obj,
        [ArgInfo(Essential = true)] Obj index)
    {
        if (self.IsFull)
            Resize(self);

        if (obj.Iter().As<Iters>(out var iters))
        {
            foreach (var item in iters.Value)
                Insert(self, item, index);
        }
        else
        {
            Insert(self, obj, index);
        }

        return self;
    }

    [Native(Name = "remove")]
    public static Bool Remove([Self] List self, [ArgInfo(Essential = true)] Obj obj)
    {
        for (int i = 0; i < self.Count; i++)
        {
            if (!self[i].Eq(obj).As<Bool>(out var eq))
                continue;                

            if (eq.Value)
                return Bool.From(RemoveAt(self, Int.From(i)).As<Bool>(out var res) && res.Value);

        }
        return Bool.False;
    }

    [Native(Name = "remove_at")]
    public static Obj RemoveAt([Self] List self, [ArgInfo(Essential = true)] Obj index)
    {
        if (!index.As<Int>(out var idx))
            return new Err("invalid arguments: index");
        if (OutOfRange(self, (int)idx.Value))
            return Bool.False;

        for (int i = (int)idx.Value; i < self.Count - 1; i++)
            self[i] = self[i + 1];
        self.Count--;
        return Bool.True;
    }

    [Native(Name = "index_of")]
    public static Int IndexOf([Self] List self, [ArgInfo(Essential = true)] Obj obj)
    {
        for (int i = 0; i < self.Count; i++)
        {
            if (!self[i].Eq(obj).As<Bool>(out var eq))
                continue;

            if (eq.Value)
                return Int.From(i);
        }
        return Int.From(-1);
    }

    [Native(Name = "contains")]
    public static Bool Contains([Self] List self, [ArgInfo(Essential = true)] Obj obj) 
        => Bool.From(IndexOf(self, obj).Value != -1);

    [Native(Name = "order")]
    public static Obj Order([Self] List self, [ArgInfo(Essential = true)] Obj fn)
    {
        if (fn is not Fn)
            return new Err("invalid arguments: fn");

        Array.Sort(self.Value, 0, self.Count, Comparer<Obj>.Create((i, j) => fn.Call(new([i], [])).CompareTo(fn.Call(new([j], [])))));
        return None;
    }

    [Native(Name = "sort")]
    public static Obj Sort([Self] List self)
    {
        Array.Sort(self.Value, 0, self.Count);
        return None;
    }

    [Native(Name = "reverse")]
    public static Obj Reverse([Self] List self)
    {
        Array.Reverse(self.Value, 0, self.Count);
        return None;
    }

    [Native(Name = "binary_search")]
    public static Int BinarySearch([Self] List self, [ArgInfo(Essential = true)] Obj obj) 
        => Int.From(Array.BinarySearch(self.Value, 0, self.Count, obj));

    [Native(Name = "lower_bound")]
    public static Obj LowerBound([Self] List self, [ArgInfo(Essential = true)] Obj obj)
    {
        int l = 0, r = self.Count - 1, m = 0;
        while (r > l)
        {
            m = (l + r) / 2;
            var lt = self[m].Lt(obj);

            if (lt.As<Bool>(out var ltValue))
                return lt is Err ? lt : new Err($"lower_bound requires '<' to return bool");

            if (ltValue.Value) l = m + 1;
            else r = m;
        }
        return Int.From(r);
    }

    [Native(Name = "upper_bound")]
    public static Obj UpperBound([Self] List self, [ArgInfo(Essential = true)] Obj obj)
    {
        int l = 0, r = self.Count - 1;
        while (r > l)
        {
            int m = (l + r) / 2;
            var gt = self[m].Gt(obj);

            if (gt.As<Bool>(out var gtValue))
                return gt is Err ? gt : new Err($"upper_bound requires '>' to return bool");

            if (gtValue.Value) l = m + 1;
            else r = m;
        }
        return Int.From(r);
    }

    [Native(Name = "hpush")]
    public static Obj HPush([Self] List self, [ArgInfo(Essential = true)] Obj obj)
    {
        int child = self.Count;
        Append(self, obj);

        while (child != 0)
        {
            int parent = (child - 1) / 2;

            if (parent < child)
                parent = child;

            child = parent;
        }

        return None;
    }

    [Native(Name = "hpop")]
    public static Obj HPop([Self] List self)
    {
        if (self.Count == 0)
            return new Err("list is empty");

        Obj value = self[0];
        self[0] = self[^1];
        self.Count--;

        int parent = 0;

        while (self.Count / 2 > parent)
        {
            int index = parent, left = 2 * parent + 1, right = 2 * parent + 2;

            if (right < self.Count && index < right)
                index = right;
            if (left < self.Count && index < left)
                index = left;

            (parent, index) = (index, parent);

            if (parent == index) break;

            parent = index;
        }

        return value;
    }

    [Native(Name = "pop")]
    public static Obj Pop([Self] List self, [ArgInfo(Essential = true)] Obj index)
    {
        if (!index.As<Int>(out var idx))
            return new Err("invalid argumnets: index");

        Obj value = self[(int)idx.Value];
        RemoveAt(self, index);
        return value;
    }

    [Native(Name = "map")]
    public static Obj Map([Self] List self, [ArgInfo(Essential = true)] Obj type)
    {
        var list = new List();
        
        foreach (var item in self)
        {
            var res = type.Call(Tup.One("", item));
            if (res is Err)
                return res;
            list.Add(res);
        }

        return list;
    }

    [Native(Name = "resize")]
    public static Obj Resize([Self] List self)
    {
        var newValue = new Obj[self.Value.Length * 2 + 1];
        for (var i = 0; i < self.Value.Length; i++)
            newValue[i] = self.Value[i];
        self.Value = newValue;
        return None;
    }
    
    public override int GetHashCode()
    {
        HashCode hash = new();

        foreach (var value in Value)
            hash.Add(value);

        return hash.ToHashCode();
    }

    public IEnumerator<Obj> GetEnumerator() => new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

}