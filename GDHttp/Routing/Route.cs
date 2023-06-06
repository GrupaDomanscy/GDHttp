namespace GDHttp.Routing;

public class Route
{
    public delegate HttpResponse Callback(HttpRequest request);
    
    public String Path { get; }
    public HttpMethod Method { get; }
    public Callback Action { get; }

    public Route(String path, HttpMethod method, Callback action)
    {
        this.Path = path;
        this.Method = method;
        this.Action = action;
    }
}