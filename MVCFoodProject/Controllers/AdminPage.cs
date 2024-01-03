using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCFoodProject.Models.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MVCFoodProject.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
            // document.cookie = "identity=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IlRlc3RVc2VyMiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6ImFkbWluIiwiZXhwIjoxNzA1MTAzMDE1LCJpc3MiOiJGb29kUHJvamVjdCIsImF1ZCI6IkpXVFRlc3QifQ.TAGiLxKyv_Wd_kR0L5wl_u6CGAUXpFtnVacLLxne7fc; path=/";
            var identity = Request.Cookies["identity"];
           
            if (identity == null)
            {
                return RedirectToAction(null, "FoodPage");
            }

             JwtSecurityToken decodedToken = new JwtSecurityToken(identity);

             var isAdmin = decodedToken.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .SingleOrDefault();

            if (isAdmin != "admin")
            {
                return RedirectToAction(null ,"Login");
            }

            var products = await _db.Products
                 .Where(p => p.IsActive == true)
                 .Include(p => p.ProductsDetails)
                 .ToListAsync();

            return View(new AdminPageViewModel { Products = products });
        }
    }
}
