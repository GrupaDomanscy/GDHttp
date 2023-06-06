using System.Net;
using GDHttp;
using GDHttp.Builders;
using GDHttp.Routing;
using HttpListener = GDHttp.HttpListener;

Router router = new Router();

router.Get("/hello-world", (request) =>
{
    HttpResponse response = new HttpResponse();
    response.Body = "Hello world";
    response.Headers.Add("Content-Type", "text/plain");
    response.StatusCode = HttpStatusCode.OK;
    return response;
});

router.Get("/", (request =>
{
    HttpResponse response = new HttpResponse();
    response.Body = "index";
    response.Headers.Add("Content-Type", "text/plain");
    response.StatusCode = HttpStatusCode.OK;
    return response;
}));

HttpListener httpListener = new HttpListener(new string[] { "http://localhost:3000/" });
httpListener.Start((HttpRequest request) =>
{
    Route? route = router.Match(request.Path, request.Method);

    if (route == null)
    {
        return new HttpResponseBuilder()
            .SetStatusCode(HttpStatusCode.NotFound)
            .SetPlainTextBody("404 Not Found")
            .Build();
    }

    return route.Action.Invoke(request);
});