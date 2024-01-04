using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCFoodProject.Models.DataBase;
using MVCFoodProject.Models.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MVCFoodProject.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    public class AdminPage : Controller
    {
        private readonly ApplicationDbContext _db;
        public AdminPage(ApplicationDbContext context)
        {
            _db = context;
        }
        

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var identity = Request.Cookies["identity"];


            if (identity == null)
            {
                return RedirectToAction(null, "FoodPage");
            }

            JwtSecurityToken decodedToken = new JwtSecurityToken(identity);

            var role = decodedToken.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .SingleOrDefault();

            if (role != "admin")
            {
                return RedirectToAction(null, "FoodPage");
            }

            var products = await _db.Products
                 .Where(p => p.IsActive == true)
                 .Include(p => p.ProductsDetails)
                 .ToListAsync();

            return View(new AdminPageViewModel { Products = products });
        }

        [AllowAnonymous]
        public async Task<object> UsersPage ()
        {

            var users = await _db.User
                .Where(u => u.Role == Users.UserRole.Customer)
                .Include(u => u.UserOrders)
                .ThenInclude(uo => uo.ProductOrders)
                .ThenInclude(po => po.Product)
                .ThenInclude(p => p.ProductsDetails)
                .ToListAsync();

            return new AdminPageViewModel { UsersList = users };
        }

    }
}
