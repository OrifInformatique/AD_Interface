using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ErrorsController : Controller
    {
        public ActionResult Dashboard()
        {
            ViewBag.Errors = ErrorModel.GetAll();
            return View();
        }
    }
}
