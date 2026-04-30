using Microsoft.AspNetCore.Mvc;

namespace Produkty24_Web.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {            
            return View();
        }
    }
}
