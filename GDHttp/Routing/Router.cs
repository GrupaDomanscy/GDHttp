namespace GDHttp.Routing;

public class Router
{
    private List<Route> _routes;

    public Router()
    {
        this._routes = new List<Route>();
    }

    public Route? Match(String path, HttpMethod method)
    {
        Route? route = _routes.Find(route =>
        {
            return route.Path == path && route.Method == method;
        });

        return route;
    }

    public void Get(String path, Route.Callback action)
    {
        _routes.Add(new Route(path, HttpMethod.Get, action));
    }
    
    public void Put(String path, Route.Callback action)
    {
        _routes.Add(new Route(path, HttpMethod.Put, action));
    }
    
    public void Post(String path, Route.Callback action)
    {
        _routes.Add(new Route(path, HttpMethod.Post, action));
    }
    
    public void Delete(String path, Route.Callback action)
    {
        _routes.Add(new Route(path, HttpMethod.Delete, action));
    }
    
    public void Head(String path, Route.Callback action)
    {
        _routes.Add(new Route(path, HttpMethod.Head, action));
    }
    
    public void Patch(String path, Route.Callback action)
    {
        _routes.Add(new Route(path, HttpMethod.Patch, action));
    }
}