using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MVCFoodProject.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "courier")]

    public class CourierPage : Controller
    {
        private readonly ApplicationDbContext _db;
        
        public CourierPage(ApplicationDbContext context)
        {
            _db = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var contextUser = (Users)HttpContext.Items["User"];

            if ((contextUser == null) || (contextUser.Role != Users.UserRole.Courier))
            {
                return RedirectToAction(null, "FoodPage");
            }

            var user = await _db.User
                .Where(u => u.Id == contextUser.Id)
                .FirstOrDefaultAsync();

            return View(new CourierPageViewModel { User = user });
        }

        [AllowAnonymous]
        public async Task<object> TakeOrdersPage()
        {
            var orders = await _db.Order
                .Where(o => o.status == Orders.Status.Created || o.status == Orders.Status.Canceled)
                .Include(o => o.ProductOrders)
                .ThenInclude(pro => pro.Product)
                .ThenInclude(pr => pr.ProductsDetails)
                .Include(o => o.User)
                .ToListAsync();

            return View(new CourierPageViewModel { Order = orders });
        }

        [AllowAnonymous]
        public async Task<object> MyTakenOrdersPage()
        {
            var contextUser = (Users)HttpContext.Items["User"];

            var courier = await _db.Courier
                .Where(c => c.User.Id == contextUser.Id)
                .FirstOrDefaultAsync();

            var orders = await _db.Order
                .Where(o => (o.status == Orders.Status.Taken) && (o.CourierId == courier.Id))
                .Include(o => o.ProductOrders)
                .ThenInclude(pro => pro.Product)
                .ThenInclude(pr => pr.ProductsDetails)
                .Include(o => o.User)
                .ToListAsync();

            return View(new CourierPageViewModel { Order = orders});
        }

        [AllowAnonymous]
        public async Task<object> HistoryOrdersPage()
        {
            var contextUser = (Users)HttpContext.Items["User"];

            var courier = await _db.Courier
                .Where(c => c.User.Id == contextUser.Id)
                .FirstOrDefaultAsync();

            var orders = await _db.Order
                .Where(o => (o.status == Orders.Status.Completed) && (o.CourierId == courier.Id))
                .Include(o => o.ProductOrders)
                .ThenInclude(pro => pro.Product)
                .ThenInclude(pr => pr.ProductsDetails)
                .Include(o => o.User)
                .ToListAsync();

            return View(new CourierPageViewModel { Order = orders });
        }
    }
}
