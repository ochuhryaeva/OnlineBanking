using System.Web.Mvc;
using OlnlineBanking.Infrastructure.Filters;

namespace OlnlineBanking
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorWithLogAttribute());
        }
    }

}