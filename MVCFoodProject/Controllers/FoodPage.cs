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
            var products = await _db.Products
                 .Where(p => p.IsActive == true)
                 .Include(p => p.ProductsDetails)
                 .ToListAsync();

            return View(new FoodPageViewModel { Products = products });
        }

        [HttpGet("/getListProductsById")]
        public async Task<List<Products>> GetListProductById(string ids)
        {
            var idsToFind = ids.Split(',').ToList();

            return await _db.Products
                 .Where(p => idsToFind.Contains(p.InternalId))
                 .Include(p => p.ProductsDetails)
                 .ToListAsync();
        }
    }
}
