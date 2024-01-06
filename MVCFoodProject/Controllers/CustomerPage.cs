using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVCFoodProject.Models.DataBase;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;

namespace MVCFoodProject.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "customer")]

    public class CustomerPage : Controller
    {
        private readonly ApplicationDbContext _db;

        public CustomerPage(ApplicationDbContext context)
        {
            _db = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var contextUser = (Users)HttpContext.Items["User"];

            if ((contextUser == null) || (contextUser.Role != Users.UserRole.Customer))
            {
                return RedirectToAction(null, "FoodPage");
            }

            var user = await _db.User
                .Where(u => u.Id == contextUser.Id)
                .FirstOrDefaultAsync();

            return View(new CustomerPageViewModel { User = user });
        }

        [AllowAnonymous]
        public async Task<object> CurrentOrdersPage()
        {
            var contextUser = (Users)HttpContext.Items["User"];

            var orders = await _db.Order
                .Where(o => (o.User.Id == contextUser.Id) && ((o.status == Orders.Status.Created) || (o.status == Orders.Status.Taken)))
                .Include(o => o.Courier)
                .ThenInclude(c => c.User)
                .Include(o => o.ProductOrders)
                .ThenInclude(o=> o.Product)
                .ThenInclude(o=> o.ProductsDetails)
                .ToListAsync();

            return View(new CustomerPageViewModel { Order = orders });
        }

        [AllowAnonymous]
        public async Task<object> AllOrdersPage()
        {
            var contextUser = (Users)HttpContext.Items["User"];

            var orders = await _db.Order
                .Where(o => o.User.Id == contextUser.Id && o.status == Orders.Status.Completed)
                .Include(o => o.Courier)
                .ThenInclude(c => c.User)
                .Include(o => o.ProductOrders)
                .ThenInclude(o => o.Product)
                .ThenInclude(o => o.ProductsDetails)
                .ToListAsync();

            return View(new CustomerPageViewModel { Order = orders});
        }
    }
}
