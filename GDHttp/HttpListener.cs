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

        ServerClosed = null;
        ServerStarted = null;
    }

    public event EventHandler? ServerClosed;
    public event EventHandler? ServerStarted;

    protected void OnServerClosed(EventArgs e)
    {
        ServerClosed?.Invoke(this, e);
    }

    protected void OnServerStarted(EventArgs e)
    {
        ServerStarted?.Invoke(this, e);
    }

    public void Start(ProcessRequestCallback callback, CancellationToken cancellationToken)
    {
        OnServerStarted(EventArgs.Empty);
        
        _httpListener.Start();
        
        while (! cancellationToken.IsCancellationRequested)
        {
            var ctx = _httpListener.GetContextAsync();

            if (! ctx.Wait(TimeSpan.FromSeconds(1)))
            {
                continue;
            }

            Task.Run(() => ProcessRequest(ctx.Result, callback));
        }
        
        OnServerClosed(EventArgs.Empty);
    }

    private void ProcessRequest(HttpListenerContext ctx, ProcessRequestCallback callback)
    {
        HttpRequest httpRequest = HttpRequest.FromHttpListenerRequest(ctx.Request);
        HttpResponse httpResponse = callback.Invoke(httpRequest);
        httpResponse.AssignToHttpListenerResponse(ctx.Response);
    }
}