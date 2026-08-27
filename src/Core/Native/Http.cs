using Un.Object;
using Un.Reflection;

namespace Un.Native;

[NativeModule("http", typeof(Object.Web.HttpClient))]
public static class Http
{
    [Native(Name = "connect")]
    public static Obj Connect()
    {
        try
        {
            return new Object.Web.HttpClient(new HttpClient());
        }
        catch (Exception e)
        {
            return new Err(e.Message);
        }
    }
}