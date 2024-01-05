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
            var contextUser = (Users)HttpContext.Items["User"];

            if ((contextUser == null) || (contextUser.Role != Users.UserRole.Admin))
            {
                return RedirectToAction(null, "FoodPage");
            }

            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> ProductsPage()
        {
            var contextUser = (Users)HttpContext.Items["User"];

            if ((contextUser == null) || (contextUser.Role != Users.UserRole.Admin))
            {
                return RedirectToAction(null, "FoodPage");
            }

            var products = await _db.Products
                 .Include(p => p.ProductsDetails)
                 .OrderBy(p => p.ProductsDetails.ProductName)
                 .ToListAsync();

            return View(new AdminPageViewModel { Products = products });
        }



        [AllowAnonymous]
        public async Task<object> UsersPage ()
        {
            var contextUser = (Users)HttpContext.Items["User"];

            if ((contextUser == null) || (contextUser.Role != Users.UserRole.Admin))
            {
                return RedirectToAction(null, "FoodPage");
            }

            var users = await _db.User
                .Where(u => u.Role == Users.UserRole.Customer)
                .Include(u => u.UserOrders)
                .ThenInclude(userOrder => userOrder.Courier)
                .Include(u => u.UserOrders)
                .ThenInclude(uo => uo.ProductOrders)
                .ThenInclude(po => po.Product)
                .ThenInclude(p => p.ProductsDetails)
                .ToListAsync();

            return View(new AdminPageViewModel { UsersList = users });
        }

    }
}
