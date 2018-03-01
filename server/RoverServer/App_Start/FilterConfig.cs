using System.Web;
using System.Web.Mvc;
using RoverServer.Filters;

namespace RoverServer
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
