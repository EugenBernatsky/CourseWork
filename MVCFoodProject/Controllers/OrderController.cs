using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Linq;

namespace MVCFoodProject.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public OrderController(ApplicationDbContext context)
        {
            _db = context;
        }

        [HttpPost("/orders")]
        public async Task<ActionResult> CreateOrder([FromBody] CreateOrderDTO DTO)
        {
            var user = (Users)HttpContext.Items["User"]!;

            var productsId = DTO.Orders.Select(x => x.ProductId).ToList();

            var products = await _db.Products.Where(p => productsId.Contains(p.InternalId) && p.Deleted != true).Include(p => p.ProductsDetails).ToListAsync();

            if (!products.SequenceEqual(products))
            {
                return NotFound();
            }

            var order = new Orders
            {
                User = user,
                TotalPrice = DTO.Orders.Select(o =>
                {
                    var currentProduct = products.Where(p => p.InternalId == o.ProductId).FirstOrDefault();

                    return o.Quantity * currentProduct?.ProductsDetails?.Price ?? 0;
                }).Sum()
            };

            var orderProducts = DTO.Orders.Select(o =>
            {
                var currentProduct = products.Where(p => p.InternalId == o.ProductId).FirstOrDefault();

                return new ProductOrders
                {
                    Product = currentProduct,
                    Quantity = o.Quantity,
                    Total = currentProduct?.ProductsDetails?.Price * o.Quantity ?? 0,
                    Order = order
                };
            }).ToList();

            try
            {
                await _db.Order.AddAsync(order);
                await _db.ProductOrder.AddRangeAsync(orderProducts);

                await _db.SaveChangesAsync();

            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
    
            return Ok(new
            {
                data = order
            });
        }
    }
}
