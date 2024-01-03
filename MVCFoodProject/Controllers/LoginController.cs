using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MVCFoodProject.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _db;
        public LoginController(ApplicationDbContext context)
        {
            _db = context;
        }

        public IActionResult Index()
        {
            var identity = Request.Cookies["identity"];

            if (identity == null)
            {
                return View();
            }

            JwtSecurityToken decodedToken = new JwtSecurityToken(identity);

            var role = decodedToken.Claims
               .Where(c => c.Type == ClaimTypes.Role)
               .Select(c => c.Value)
               .SingleOrDefault();

            if (role == "admin")
            {
                return RedirectToAction(null, "AdminPage");
            }

            // TODO
            if (role == "customer")
            {
                return RedirectToAction(null, "FoodPage");
            }

            if (role == "courier")
            {
                return RedirectToAction(null, "FoodPage");
            }

            return View();
        }
    }
}
