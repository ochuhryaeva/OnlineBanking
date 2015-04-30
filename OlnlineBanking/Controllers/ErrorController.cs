using System.Web.Mvc;

namespace OlnlineBanking.Controllers
{
    public class ErrorController : Controller
    {
        //404 Error Handle
        public ActionResult PageNotFound()
        {
            if (Response != null)
            {
                Response.StatusCode = 404;
                Response.TrySkipIisCustomErrors = true;    
            }
            return View("PageNotFound");
        }
    }
}
