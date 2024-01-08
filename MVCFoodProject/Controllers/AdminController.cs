using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVCFoodProject.Models.DataBase;

namespace MVCFoodProject.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public AdminController(ApplicationDbContext context)
        {
            _db = context;
        }

        [HttpPost("/products/toggle")]
        public async Task<ActionResult<Products>> ToggleProdutc([FromBody] ToggleProductDTO body)
        {
            var product = await _db.Products
                .Where(p => p.InternalId == body.internalId && p.Deleted != true)
                .Include(p => p.ProductsDetails)
                .FirstOrDefaultAsync();

            try
            {
                product.IsActive = body.action == ToggleProductDTO.ACTION.activate ? true : false;

                await _db.SaveChangesAsync();

            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            
            return Ok(product);
        }


        [HttpPost("/products/product/edit/{id}")]
        public async Task<ActionResult> EditProduct ([FromRoute] string id, [FromForm] EditProductDTO body)
        {
            var product = await _db.Products.Where(p => p.InternalId == id && p.Deleted != true)
                .Include(p => p.ProductsDetails)    
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            try
            {
                if (body.image != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await body.image.CopyToAsync(memoryStream);
                        var imageBytes = memoryStream.ToArray();
                        var base64String = Convert.ToBase64String(imageBytes);

                        product.ProductsDetails.imgURL = $"data:{body.image.ContentType};base64, {base64String}";
                    }
                }

                if (body.productName != null)
                {
                    product.ProductsDetails.ProductName = body.productName;
                }

                if (body.description != null)
                {
                    product.ProductsDetails.Description = body.description;
                }

                if (body.price != null)
                {
                    product.ProductsDetails.Price = (int)body.price;
                }

                if (_db.ChangeTracker.HasChanges())
                {
                    await _db.SaveChangesAsync();
                }

            } catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex.Message);
            }


                return Ok(product);
        }

        [HttpGet("/products/product/{id}")]
        public async Task<ActionResult<String>> GetProductById([FromRoute] string id)
        {
            var product = await _db.Products
                        .Where(p => p.InternalId == id && p.Deleted != true)
                        .Include(p => p.ProductsDetails)
                        .FirstOrDefaultAsync();
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpDelete("/products/{id}")]
        public async Task<ActionResult> DeleteProduct([FromRoute] string id)
        {
            var product = await _db.Products.Where(p => p.InternalId == id).FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound($"Product with id {id} not found");
            }

            try
            {
                product.Deleted = true;
                _db.SaveChanges();
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return NoContent();
        }

        [HttpPost("/users/toggle-role")]
        public async Task<ActionResult> ToggleRole([FromBody] ToggleUserRoleDTO body)
        {
            if (body.action == ToggleUserRoleDTO.ACTION.role_courier)
            {
                var user = await _db.User
                .Where(u => u.Id == body.Id)
                .FirstOrDefaultAsync();

                try
                {
                    if (user != null)
                    {
                        user.Role = Users.UserRole.Courier;

                        var courier = new Courier();
                        courier.User = user;

                        _db.Courier.Add(courier);
                    }

                    if (_db.ChangeTracker.HasChanges())
                    {
                        await _db.SaveChangesAsync();
                    }

                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            if (body.action == ToggleUserRoleDTO.ACTION.role_customer)
            {
                var courier = await _db.Courier
                    .Where(c => c.Id == body.Id)
                    .Include(c => c.User)
                    .FirstOrDefaultAsync();

                try
                {
                    if (courier.Order.Any())
                    {
                        return Ok(new { HasOrders = true });
                    }

                    if (courier != null && !courier.Order.Any())
                    {
                        courier.User.Role = Users.UserRole.Customer;

                        _db.Courier.Remove(courier);
                    }

                    if (_db.ChangeTracker.HasChanges())
                    {
                        await _db.SaveChangesAsync();
                    }

                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }


            return Ok();
        }

    }
}
