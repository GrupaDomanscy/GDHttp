using System.Collections.Specialized;
using System.Net;
using System.Reflection;
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

    public T ParseQueryParamsToClass<T>()
    {
        Type type = typeof(T);

        T instance = (T)Activator.CreateInstance(type)!;

        for (var i = 0; i < type.GetProperties().Length; i++)
        {
            PropertyInfo propertyInfo = type.GetProperties()[i];

            String propertyName = propertyInfo.Name;
            Type propertyType = propertyInfo.PropertyType;

            if (!QueryParams.ContainsKey(propertyName)) continue;
            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType)!;
            }

            if (propertyType == typeof(string))
            {
                propertyInfo.SetValue(instance, QueryParams[propertyName]);
            }
            else
            {
                try
                {
                    propertyInfo.SetValue(instance, Convert.ChangeType(QueryParams[propertyName], propertyType));
                }
                catch (InvalidCastException)
                {
                    // This error is thrown when trying to convert string to other incompatible type.
                    // It happens when client provided impossible to convert (wrong) data.
                }
                catch (FormatException)
                {
                    // This error is thrown when trying to convert string to other compatible type, but wrong format.
                    // It happens when client provided impossible to convert (wrong) data.
                }
                catch (OverflowException)
                {
                    // This error is thrown when trying to convert string to other compatible type, but final type is
                    // too small for the provided input. (for ex. int32 can't be bigger than 2,147,483,647)
                    // It happens when client provided impossible to convert (wrong) data.
                }
            }
        }

        return instance;
    }

    public static HttpRequest FromHttpListenerRequest(HttpListenerRequest source)
    {
        string path = source.Url!.AbsolutePath;

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