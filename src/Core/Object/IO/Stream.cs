using Un.Object.Primitive;
using Un.Object.Type;
using Un.Reflection;

namespace Un.Object.IO;

public class Stream : Ref<System.IO.Stream>, IDisposable
{
    public StreamReader? Reader { get; }
    public StreamWriter? Writer { get; }

    public bool CanRead => Value.CanRead;
    public bool CanWrite => Value.CanWrite;

    public Stream(System.IO.Stream stream) : base(stream, UnType.Create("stream"))
    {
        if (stream.CanRead)
            Reader = new StreamReader(stream, leaveOpen: true);
        if (stream.CanWrite)
            Writer = new StreamWriter(stream, leaveOpen: true)
            {
                AutoFlush = false
            };
    }

    public override Obj Entry() => None;

    public override Obj Exit()
    {
        Close();
        return None; 
    }

    public void Close()
    {
        Dispose();
    }

    public void Dispose()
    {
        Writer?.Dispose();
        Reader?.Dispose();
        Value.Dispose();

        GC.SuppressFinalize(this);
    }

    [Native(Name = "read")]
    public static Obj Read([Self] Stream self)
    {
        if (!self.CanRead)
            return new Err("stream is not readable");

        return Str.From(self.Reader!.ReadToEnd());
    }

    [Native(Name = "read_line")]
    public static Obj ReadLine([Self] Stream self)
    {
        if (!self.CanRead)
            return new Err("stream is not readable");

        var line = self.Reader!.ReadLine();
        return line is null ? None : Str.From(line);
    }

    [Native(Name = "write")]
    public static Obj Write([Self] Stream self, [ArgInfo(Essential = true)] Obj value)
    {
        if (!self.CanWrite)
            return new Err("stream is not writable");

        self.Writer!.Write(Str.To(value).Value);
        return None;
    }

    [Native(Name = "write_line")]
    public static Obj WriteLine(
        [Self] Stream self,
        [ArgInfo(Essential = true)] Obj value)
    {
        if (!self.CanWrite)
            return new Err("stream is not writable");

        self.Writer!.WriteLine(Str.To(value).Value);
        return None;
    }

    [Native(Name = "flush")]
    public static Obj Flush([Self] Stream self)
    {
        if (!self.CanWrite)
            return new Err("stream is not writable");

        self.Writer!.Flush();
        return None;
    }

    [Native(Name = "close")]
    public static Obj Close([Self] Stream self)
    {
        self.Close();
        return None;
    }

    [Native(Name = "seek")]
    public static Obj Seek(
        [Self] Stream self,
        [ArgInfo(Essential = true)] Int position)
    {
        if (!self.Value.CanSeek)
            return new Err("stream is not seekable");

        self.Value.Seek(position.Value, SeekOrigin.Begin);
        return None;
    }

    [Native(Name = "position")]
    public static Obj Position([Self] Stream self)
    {
        if (!self.Value.CanSeek)
            return new Err("stream is not seekable");

        return Int.From(self.Value.Position);
    }

    [Native(Name = "set_position")]
    public static Obj SetPosition(
        [Self] Stream self,
        [ArgInfo(Essential = true)] Int position)
    {
        if (!self.Value.CanSeek)
            return new Err("stream is not seekable");

        self.Value.Position = position.Value;
        return None;
    }

    [Native(Name = "length")]
    public static Obj Length([Self] Stream self)
    {
        if (!self.Value.CanSeek)
            return new Err("stream is not seekable");

        return Int.From(self.Value.Length);
    }

    [Native(Name = "eof")]
    public static Obj EndOfFile([Self] Stream self)
    {
        if (!self.CanRead)
            return new Err("stream is not readable");

        return Bool.From(self.Reader!.EndOfStream);
    }

    [Native(Name = "can_read")]
    public static Bool _CanRead([Self] Stream self) => Bool.From(self.Value.CanRead);

    [Native(Name = "can_write")]
    public static Bool _CanWrite([Self] Stream self) => Bool.From(self.Value.CanWrite);

    [Native(Name = "can_seek")]
    public static Bool CanSeek([Self] Stream self) => Bool.From(self.Value.CanSeek);
}