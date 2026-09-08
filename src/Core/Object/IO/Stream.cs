using Un.Object.Primitive;
using Un.Object.Type;
using Un.Reflection;

namespace Un.Object.IO;

[NativeType("stream", Description = "Stream for file I/O operations.", Example = "using file = io.open(\"test.md\", \"w\")\nfile.write(\"hi\")")]
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

    [Native(
        Name = "read",
        Description = "Reads all remaining text from a stream.",
        Example = "stream.read()",
        ReturnType = "str"
    )]
    public static Obj Read([Self] Stream self)
    {
        if (!self.CanRead)
            return new Err("stream is not readable");

        return Str.From(self.Reader!.ReadToEnd());
    }

    [Native(
        Name = "read_line",
        Description = "Reads the next line from a stream.",
        Example = "line = stream.read_line()",
        ReturnType = "str"
    )]
    public static Obj ReadLine([Self] Stream self)
    {
        if (!self.CanRead)
            return new Err("stream is not readable");

        var line = self.Reader!.ReadLine();
        return line is null ? None : Str.From(line);
    }

    [Native(
        Name = "write",
        Description = "Writes a value to a stream without a trailing line break.",
        Example = "stream.write(value)",
        ReturnType = "none",
        ArgumentTypes = new[] { "any" }
    )]
    public static Obj Write([Self] Stream self, [ArgInfo(Essential = true)] Obj value)
    {
        if (!self.CanWrite)
            return new Err("stream is not writable");

        self.Writer!.Write(Str.To(value).Value);
        return None;
    }

    [Native(
        Name = "write_line",
        Description = "Writes a value and a trailing line break to a stream.",
        Example = "stream.write_line(value)",
        ReturnType = "none",
        ArgumentTypes = new[] { "any" }
    )]
    public static Obj WriteLine(
        [Self] Stream self,
        [ArgInfo(Essential = true)] Obj value)
    {
        if (!self.CanWrite)
            return new Err("stream is not writable");

        self.Writer!.WriteLine(Str.To(value).Value);
        return None;
    }

    [Native(
        Name = "flush",
        Description = "Flushes buffered stream output.",
        Example = "stream.flush()",
        ReturnType = "none"
    )]
    public static Obj Flush([Self] Stream self)
    {
        if (!self.CanWrite)
            return new Err("stream is not writable");

        self.Writer!.Flush();
        return None;
    }

    [Native(
        Name = "close",
        Description = "Closes and disposes a stream.",
        Example = "stream.close()",
        ReturnType = "none"
    )]
    public static Obj Close([Self] Stream self)
    {
        self.Close();
        return None;
    }

    [Native(
        Name = "seek",
        Description = "Seeks to an absolute byte position in a stream.",
        Example = "stream.seek(0)",
        ReturnType = "none",
        ArgumentTypes = new[] { "int" }
    )]
    public static Obj Seek(
        [Self] Stream self,
        [ArgInfo(Essential = true)] Int position)
    {
        if (!self.Value.CanSeek)
            return new Err("stream is not seekable");

        self.Value.Seek(position.Value, SeekOrigin.Begin);
        return None;
    }

    [Native(
        Name = "position",
        Description = "Returns the current byte position in a stream.",
        Example = "write(stream.position())",
        ReturnType = "int"
    )]
    public static Obj Position([Self] Stream self)
    {
        if (!self.Value.CanSeek)
            return new Err("stream is not seekable");

        return Int.From(self.Value.Position);
    }

    [Native(
        Name = "set_position",
        Description = "Sets the current byte position in a stream.",
        Example = "stream.set_position(0)",
        ReturnType = "none",
        ArgumentTypes = new[] { "int" }
    )]
    public static Obj SetPosition(
        [Self] Stream self,
        [ArgInfo(Essential = true)] Int position)
    {
        if (!self.Value.CanSeek)
            return new Err("stream is not seekable");

        self.Value.Position = position.Value;
        return None;
    }

    [Native(
        Name = "length",
        Description = "Returns the stream length in bytes.",
        Example = "write(stream.length())",
        ReturnType = "int"
    )]
    public static Obj Length([Self] Stream self)
    {
        if (!self.Value.CanSeek)
            return new Err("stream is not seekable");

        return Int.From(self.Value.Length);
    }

    [Native(
        Name = "eof",
        Description = "Checks whether a readable stream reached end of file.",
        Example = "write(stream.eof())",
        ReturnType = "bool"
    )]
    public static Obj EndOfFile([Self] Stream self)
    {
        if (!self.CanRead)
            return new Err("stream is not readable");

        return Bool.From(self.Reader!.EndOfStream);
    }

    [Native(
        Name = "can_read",
        Description = "Checks whether a stream supports reading.",
        Example = "write(stream.can_read())",
        ReturnType = "bool"
    )]
    public static Bool _CanRead([Self] Stream self) => Bool.From(self.Value.CanRead);

    [Native(
        Name = "can_write",
        Description = "Checks whether a stream supports writing.",
        Example = "write(stream.can_write())",
        ReturnType = "bool"
    )]
    public static Bool _CanWrite([Self] Stream self) => Bool.From(self.Value.CanWrite);

    [Native(
        Name = "can_seek",
        Description = "Checks whether a stream supports seeking.",
        Example = "write(stream.can_seek())",
        ReturnType = "bool"
    )]
    public static Bool CanSeek([Self] Stream self) => Bool.From(self.Value.CanSeek);
}
