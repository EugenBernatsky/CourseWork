using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCFoodProject.Models.DataBase;
using MVCFoodProject.Models.ViewModels;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Xml.Linq;

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

            var me = await _db.User.Where(u => u.Id == contextUser.Id).FirstOrDefaultAsync();

            return View(new AdminPageViewModel { Me = me! });
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
                  .Where(p => p.Deleted != true)
                  .Include(p => p.ProductsDetails)
                  .OrderBy(p => p.ProductsDetails.ProductName)
                  .ToListAsync();

            return View(new AdminPageViewModel { Products = products });
        }

        [AllowAnonymous] 
        public async Task<IActionResult> OrderPage (int orderID)
        {
            var contextUser = (Users)HttpContext.Items["User"];

            if ((contextUser == null) || (contextUser.Role != Users.UserRole.Admin))
            {
                return RedirectToAction(null, "FoodPage");
            }

            var order = await _db.Order
                .Where(o => o.Id == orderID)
                .Include(o => o.ProductOrders)
                .ThenInclude(p => p.Product)
                .ThenInclude(po => po!.ProductsDetails)
                .Include(o => o.Courier)
                .ThenInclude(c => c.User)
                .Include(o => o.User)
                .Select(f => new Orders
                {
                    Id = f.Id,
                    CreatedDate = f.CreatedDate,
                    User = new Users
                    {
                        Id = f.User.Id,
                        Name = f.User.Name,
                        imgURL = f.User.imgURL,
                        Number = f.User.Number,
                        Adress = f.User.Adress,
                        Role = f.User.Role

                    },
                    Courier = f.Courier != null ? new Courier
                    {
                        Id= f.Courier.Id,
                        status = f.Courier.status,
                        User = f.Courier.User

                    } : null,
                    ProductOrders = f.ProductOrders,
                    TotalPrice = f.TotalPrice,
                    status = f.status
                })
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound($"Order {orderID} not found");
            }

            return View(new OrderViewModel(order));
        }

        [AllowAnonymous]
        public async Task<ActionResult> CouriersPage ()
        {
            var contextUser = (Users)HttpContext.Items["User"];

            if ((contextUser == null) || (contextUser.Role != Users.UserRole.Admin))
            {
                return RedirectToAction(null, "FoodPage");
            }

            var couriers = await _db.Courier
                .Include(c => c.Order.OrderBy(o => o.CreatedDate))
                .Include(c => c.User)
                .ToListAsync();

            return View(new AdminPageViewModel { Couriers = couriers});
        }

        [AllowAnonymous]
        public async Task<ActionResult> UsersPage ()
        {
            var contextUser = (Users)HttpContext.Items["User"];

            if ((contextUser == null) || (contextUser.Role != Users.UserRole.Admin))
            {
                return RedirectToAction(null, "FoodPage");
            }

            var users = await _db.User
                //.Where(u => u.Role == Users.UserRole.Customer)
                .Include(u => u.UserOrders)
                .ThenInclude(userOrder => userOrder.Courier)
                .ThenInclude(c => c.User)
                .Include(u => u.UserOrders)
                .ThenInclude(uo => uo.ProductOrders)
                .ThenInclude(po => po.Product)
                .ThenInclude(p => p.ProductsDetails)
                .ToListAsync();

            return View(new AdminPageViewModel { UsersList = users });
        }

    }
}
