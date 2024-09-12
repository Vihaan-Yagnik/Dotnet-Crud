using Microsoft.AspNetCore.Mvc;

namespace Admin3.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
