using System.Net;
using System.Text;
using GDHttp.Builders;

namespace GDHttp;

public class HttpResponse
{
    /* SOURCE: https://referencesource.microsoft.com/#System.Web/WorkerRequest.cs,afb948c97b3d42e4, */
    private static readonly String[][] _HTTPStatusDescriptions =
    {
        null,

        new String[]
        {
            /* 100 */"Continue",
            /* 101 */ "Switching Protocols",
            /* 102 */ "Processing"
        },

        new String[]
        {
            /* 200 */"OK",
            /* 201 */ "Created",
            /* 202 */ "Accepted",
            /* 203 */ "Non-Authoritative Information",
            /* 204 */ "No Content",
            /* 205 */ "Reset Content",
            /* 206 */ "Partial Content",
            /* 207 */ "Multi-Status"
        },

        new String[]
        {
            /* 300 */"Multiple Choices",
            /* 301 */ "Moved Permanently",
            /* 302 */ "Found",
            /* 303 */ "See Other",
            /* 304 */ "Not Modified",
            /* 305 */ "Use Proxy",
            /* 306 */ String.Empty,
            /* 307 */ "Temporary Redirect"
        },

        new String[]
        {
            /* 400 */"Bad Request",
            /* 401 */ "Unauthorized",
            /* 402 */ "Payment Required",
            /* 403 */ "Forbidden",
            /* 404 */ "Not Found",
            /* 405 */ "Method Not Allowed",
            /* 406 */ "Not Acceptable",
            /* 407 */ "Proxy Authentication Required",
            /* 408 */ "Request Timeout",
            /* 409 */ "Conflict",
            /* 410 */ "Gone",
            /* 411 */ "Length Required",
            /* 412 */ "Precondition Failed",
            /* 413 */ "Request Entity Too Large",
            /* 414 */ "Request-Uri Too Long",
            /* 415 */ "Unsupported Media Type",
            /* 416 */ "Requested Range Not Satisfiable",
            /* 417 */ "Expectation Failed",
            /* 418 */ String.Empty,
            /* 419 */ String.Empty,
            /* 420 */ String.Empty,
            /* 421 */ String.Empty,
            /* 422 */ "Unprocessable Entity",
            /* 423 */ "Locked",
            /* 424 */ "Failed Dependency"
        },
        new String[]
        {
            /* 500 */"Internal Server Error",
            /* 501 */ "Not Implemented",
            /* 502 */ "Bad Gateway",
            /* 503 */ "Service Unavailable",
            /* 504 */ "Gateway Timeout",
            /* 505 */ "Http Version Not Supported",
            /* 506 */ String.Empty,
            /* 507 */ "Insufficient Storage"
        }
    };

    /* SOURCE: https://referencesource.microsoft.com/#System.Web/WorkerRequest.cs,4cc10728bf0adf5d */
    private static string GetStatusDescription(int code)
    {
        if (code >= 100 && code < 600)
        {
            int i = code / 100;
            int j = code % 100;

            if (j < _HTTPStatusDescriptions[i].Length)
                return _HTTPStatusDescriptions[i][j];
        }

        return String.Empty;
    }

    public HttpStatusCode StatusCode { get; set; }
    public string Body { get; set; }
    public Dictionary<string, string> Headers { get; set; }

    public HttpResponse()
    {
        this.Body = "";
        this.Headers = new Dictionary<string, string>();
    }

    public static HttpResponse EmptyNotFound()
    {
        return new HttpResponseBuilder()
            .SetStatusCode(HttpStatusCode.NotFound)
            .SetPlainTextBody("")
            .Build();
    }
    
    public void AssignToHttpListenerResponse(HttpListenerResponse response)
    {
        response.StatusCode = (int) this.StatusCode;
        response.StatusDescription = GetStatusDescription(response.StatusCode);

        foreach (string key in this.Headers.Keys)
        {
            response.Headers.Add(key, this.Headers[key]);
        }
        
        byte[] buffer = Encoding.UTF8.GetBytes(Body);
        response.ContentLength64 = buffer.Length;
        
        Stream ros = response.OutputStream;
        ros.Write(buffer, 0, buffer.Length);
        ros.Close();
    }
}