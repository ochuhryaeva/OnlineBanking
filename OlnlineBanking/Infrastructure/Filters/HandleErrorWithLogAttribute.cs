using System.Reflection;
using System.Text;
using System.Web.Mvc;
using log4net;

namespace OlnlineBanking.Infrastructure.Filters
{
    public class HandleErrorWithLogAttribute: HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            //logging exception
            ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            StringBuilder errorDescription = new StringBuilder();
            errorDescription.AppendLine();
            errorDescription.AppendLine("exception message:" + filterContext.Exception.Message);
            string controllerName = (string) filterContext.RouteData.Values["controller"];
            string actionName = (string)filterContext.RouteData.Values["action"];
            errorDescription.AppendLine("controller: "+controllerName);
            errorDescription.AppendLine("action: " + actionName);
            errorDescription.AppendLine("stack trace:");
            errorDescription.AppendLine(filterContext.Exception.StackTrace);
            logger.Error(errorDescription.ToString());
            base.OnException(filterContext);
        }
    }
}