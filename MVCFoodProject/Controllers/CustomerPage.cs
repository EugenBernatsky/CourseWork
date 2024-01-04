using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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

            if (role != "customer")
            {
                return RedirectToAction(null, "FoodPage");
            }

            var userId = decodedToken.Claims
            .Where(c => c.Type == ClaimTypes.NameIdentifier)
            .Select(c => c.Value)
            .SingleOrDefault();

            
            var user = await _db.User
                .Where(u => u.UID == userId)
                .FirstOrDefaultAsync();

            return View(new CustomerPageViewModel { User = user });
        }

        [AllowAnonymous]
        public async Task<object> CurrentOrdersPage()
        {
            var identity = Request.Cookies["identity"];

            if (identity != null)
            {
                JwtSecurityToken decodedToken = new JwtSecurityToken(identity);

                var userId = decodedToken.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value)
                    .SingleOrDefault();

                if (!string.IsNullOrEmpty(userId))
                {
                    var user = await _db.User
                        .Where(u => u.UID == userId)
                        .FirstOrDefaultAsync();

                    if (user != null)
                    {
                        var orders = await _db.Order
                            .Where(o => o.User.Id == user.Id)
                            .ToListAsync();

                        return new CustomerPageViewModel { Order = orders };
                    }
                }
            }
            return View(new CustomerPageViewModel { Order = new List<Orders>() });
        }

        [AllowAnonymous]
        public async Task<object> AllOrdersPage()
        {
            var identity = Request.Cookies["identity"];

            if (identity != null)
            {
                JwtSecurityToken decodedToken = new JwtSecurityToken(identity);

                var userId = decodedToken.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value)
                    .SingleOrDefault();

                if (!string.IsNullOrEmpty(userId))
                {
                    var user = await _db.User
                        .Where(u => u.UID == userId)
                        .FirstOrDefaultAsync();

                    if (user != null)
                    {
                        var orders = await _db.Order
                            .Where(o => o.User.Id == user.Id)
                            .ToListAsync();

                        return new CustomerPageViewModel { Order = orders };
                    }
                }
            }
            return View(new CustomerPageViewModel { Order = new List<Orders>() });
        }
    }
}
