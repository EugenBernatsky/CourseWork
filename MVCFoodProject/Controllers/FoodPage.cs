using Microsoft.AspNetCore.Mvc;

namespace MVCFoodProject.Controllers
{
    public class FoodPage : Controller
    {
        private readonly ApplicationDbContext _db;
        public FoodPage(ApplicationDbContext context) {
            _db = context;
        }
        public async Task<IActionResult> Index()
        {
            var contextUser = (Users)HttpContext.Items["User"];
            UserProfile? userProfile = null;

            if (contextUser != null)
            {
                userProfile = new UserProfile
                {
                    userId = contextUser.UID,
                    role = contextUser.Role.ToString().ToLower()
                };
            }

            var products = await _db.Products
                 .Where(p => p.IsActive == true && p.Deleted != true)
                 .Include(p => p.ProductsDetails)
                 .ToListAsync();

            return View(new FoodPageViewModel { Products = products, UserProfile = userProfile });
        }

        [HttpGet("/getListProductsById")]
        public async Task<List<Products>> GetListProductById(string ids)
        {
            var idsToFind = ids.Split(',').ToList();

            return await _db.Products
                 .Where(p => idsToFind.Contains(p.InternalId) && p.Deleted != true)
                 .Include(p => p.ProductsDetails)
                 .ToListAsync();
        }

        [HttpGet("/products")]
        public async Task<ActionResult> ProductsList(string? sort, string? field, string? category)
        {
            var product = _db.Products.AsQueryable(); ;

            if (category != null)
            {
                product = product.Where(p => p.CategoryType == category && p.Deleted != true && p.IsActive == true);
            } else
            {
                product = product.Where(p => p.Deleted != true && p.IsActive == true);
            }

            if (sort == "ASC" && field == "price")
            {
                product = product.OrderBy(p => p.ProductsDetails.Price);
            }

            if (sort == "DESC" && field == "price")
            {
                product = product.OrderByDescending(p => p.ProductsDetails.Price);
            }

            if (sort == "ASC" && field == "name")
            {
                product = product.OrderBy(p => p.ProductsDetails.ProductName);
            }

            if (sort == "DESC" && field == "name")
            {
                product = product.OrderByDescending(p => p.ProductsDetails.ProductName);
            }




            var result = await product.Include(p => p.ProductsDetails).ToListAsync();

            return Ok(result);
        }
    }
}
