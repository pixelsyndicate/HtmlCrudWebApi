using System.Web.Http;

namespace HTMLTableSamples
{
  public static class WebApiConfig
  {
    public static void Register(HttpConfiguration config)
    {
      // Clear any old routes
      config.Routes.Clear();

      // Add route for our API calls
      config.Routes.MapHttpRoute(
          name: "DefaultApi",
          routeTemplate: "api/{controller}/{id}",
          defaults: new { id = RouteParameter.Optional }
      );
    }
  }
}
