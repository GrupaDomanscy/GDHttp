namespace GDHttp.Routing;

public class Router
{
    private List<Route> _routes;

    public Router()
    {
        this._routes = new List<Route>();
    }

    public Route? Match(string path, HttpMethod method)
    {
        Route? route = _routes.Find(route =>
        {
            return route.Path == path && route.Method == method;
        });

        return route;
    }

    public void Get(string path, Route.Callback action)
    {
        _routes.Add(new Route(path, HttpMethod.Get, action));
    }
    
    public void Put(string path, Route.Callback action)
    {
        _routes.Add(new Route(path, HttpMethod.Put, action));
    }
    
    public void Post(string path, Route.Callback action)
    {
        _routes.Add(new Route(path, HttpMethod.Post, action));
    }
    
    public void Delete(string path, Route.Callback action)
    {
        _routes.Add(new Route(path, HttpMethod.Delete, action));
    }
    
    public void Head(string path, Route.Callback action)
    {
        _routes.Add(new Route(path, HttpMethod.Head, action));
    }
    
    public void Patch(string path, Route.Callback action)
    {
        _routes.Add(new Route(path, HttpMethod.Patch, action));
    }
}