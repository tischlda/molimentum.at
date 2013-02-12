using System.Web.Mvc;
using Raven.Client;

namespace Molimentum.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Impressum()
        {
            return View();
        }
    }
}
