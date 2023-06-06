using System.Net;

namespace GDHttp;

public class HttpListener
{
    private System.Net.HttpListener _httpListener;
    
    public delegate HttpResponse ProcessRequestCallback(HttpRequest httpRequest);
    
    public HttpListener(string[] domains)
    {
        this._httpListener = new System.Net.HttpListener();
        
        foreach (string domain in domains)
        {
            this._httpListener.Prefixes.Add(domain);
        }
    }

    public void Start(ProcessRequestCallback callback)
    {
        _httpListener.Start();
        
        while (true)
        {
            HttpListenerContext ctx = _httpListener.GetContext();

            Task.Run(() => ProcessRequest(ctx, callback));
        }
    }

    private void ProcessRequest(HttpListenerContext ctx, ProcessRequestCallback callback)
    {
        HttpRequest httpRequest = HttpRequest.FromHttpListenerRequest(ctx.Request);
        HttpResponse httpResponse = callback.Invoke(httpRequest);
        httpResponse.AssignToHttpListenerResponse(ctx.Response);
    }
}