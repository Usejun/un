using System.Net.WebSockets;
using Un.Object;
using Un.Object.Primitive;
using Un.Reflection;

namespace Un.Native;

[NativeModule("ws", typeof(Object.Web.WebSocket))]
public static class Ws
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

    [Native(Name = "connect")]
    public static Obj Connect([ArgInfo(Essential = true)] Obj url)
    {
        if (!GetString(url, out var u, out var err))
            return err;

        try
        {
            var client = new ClientWebSocket();
            client.ConnectAsync(new Uri(u), CancellationToken.None).GetAwaiter().GetResult();
            return new Object.Web.WebSocket(client);
        }
        catch (Exception e)
        {
            return new Err(e.Message);
        }
    }
}