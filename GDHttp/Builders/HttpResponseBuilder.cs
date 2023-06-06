using System.Net;
using System.Text.Json;

namespace GDHttp.Builders;

public class HttpResponseBuilder
{
    private string _body;
    private Dictionary<string, string> _headers;
    private HttpStatusCode _statusCode;

    public HttpResponseBuilder()
    {
        _statusCode = HttpStatusCode.OK;
        _body = "";
        this._headers = new Dictionary<string, string>();
    }
    
    public HttpResponseBuilder SetJsonBody(Object obj)
    {
        _body = JsonSerializer.Serialize(obj);
        _headers.Add("Content-Type", "application/json");
        return this;
    }

    public HttpResponseBuilder SetPlainTextBody(string body)
    {
        _body = body;
        _headers.Add("Content-Type", "text/plain");
        return this;
    }

    public HttpResponseBuilder AddHeader(string key, string value)
    {
        _headers.Add(key, value);
        return this;
    }

    public HttpResponseBuilder SetStatusCode(HttpStatusCode statusCode)
    {
        _statusCode = statusCode;
        return this;
    }

    public HttpResponse Build()
    {
        HttpResponse httpResponse = new HttpResponse();
        httpResponse.Body = _body;
        httpResponse.Headers = _headers;
        httpResponse.StatusCode = _statusCode;
        return httpResponse;
    }
}