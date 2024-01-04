using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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

            if (role != "courier")
            {
                return RedirectToAction(null, "CourierPage");
            }

            var userId = decodedToken.Claims
            .Where(c => c.Type == ClaimTypes.NameIdentifier)
            .Select(c => c.Value)
            .SingleOrDefault();

            var courier = await _db.Courier
                .Where(c => c.User.UID == userId)
                .FirstOrDefaultAsync();

            return View(new CourierPageViewModel { Courier = courier });
        }

        [AllowAnonymous]
        public async Task<object> TakeOrdersPage()
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
            return View(new CourierPageViewModel { Order = new List<Orders>() });
        }
    }
}
