using Microsoft.AspNetCore.Mvc;

namespace MVCFoodProject.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
