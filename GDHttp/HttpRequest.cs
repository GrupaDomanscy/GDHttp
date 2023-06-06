using System.Collections.Specialized;
using System.Net;
using System.Web;

namespace GDHttp;

public class HttpRequest
{
    public HttpMethod Method { get; private set; }
    public string Path { get; private set; }
    public Dictionary<string, string> Headers { get; private set; }
    public Dictionary<string, string> QueryParams { get; private set; }
    public string Body { get; private set; }

    public HttpRequest(
        string path,
        HttpMethod method,
        Dictionary<string, string> headers,
        Dictionary<string, string> queryParams,
        string body
    )
    {
        this.Method = method;
        this.Path = path;
        this.Headers = headers;
        this.QueryParams = queryParams;
        this.Body = body;
    }

    public static HttpRequest FromHttpListenerRequest(HttpListenerRequest source)
    {
        String path = source.Url!.AbsolutePath;

        string[] headerKeys = source.Headers.AllKeys!;

        Dictionary<string, string> headers = new Dictionary<string, string>();

        foreach (string key in headerKeys)
        {
            headers.Add(key, source.Headers.Get(key)!);
        }

        HttpMethod method;

        switch (source.HttpMethod)
        {
            case "GET":
                method = HttpMethod.Get;
                break;
            case "POST":
                method = HttpMethod.Post;
                break;
            case "CONNECT":
                method = HttpMethod.Connect;
                break;
            case "DELETE":
                method = HttpMethod.Delete;
                break;
            case "PATCH":
                method = HttpMethod.Patch;
                break;
            case "HEAD":
                method = HttpMethod.Head;
                break;
            case "OPTIONS":
                method = HttpMethod.Options;
                break;
            case "PUT":
                method = HttpMethod.Put;
                break;
            case "TRACE":
                method = HttpMethod.Trace;
                break;
            default:
                throw new Exception("Invalid method");
        }

        Dictionary<string, string> queryParams = new Dictionary<string, string>();

        NameValueCollection queryString = HttpUtility.ParseQueryString(source.Url.Query);
        foreach (string key in queryString.AllKeys)
        {
            queryParams.Add(key!, queryString.Get(key)!);
        }

        StreamReader bodyReader = new StreamReader(source.InputStream, source.ContentEncoding);

        return new HttpRequest(path, method, headers, queryParams, bodyReader.ReadToEnd());
    }
}