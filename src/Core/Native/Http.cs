using Un.Object;
using Un.Reflection;

namespace Un.Native;

[NativeModule("http", typeof(Object.Web.HttpClient))]
public static class Http
{
    [Native(
        Name = "connect",
        Description = "Creates an HTTP client object.",
        Example = "client = connect()",
        ReturnType = "http_client"
    )]
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
