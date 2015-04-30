using System.Web.Mvc;
using NUnit.Framework;
using OlnlineBanking.Controllers;

namespace OlnlineBanking.Tests
{
    [TestFixture]
    public class ErrorControllerTests
    {
        [Test]
        public void PageNotFound_ShowViewWithErrorPageNotFound()
        {
            ErrorController controller = new ErrorController();
            ActionResult result = controller.PageNotFound();
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual("pagenotfound",((ViewResult)result).ViewName.ToLower());
        }
    }
}
