using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVCFoodProject.Models.DataBase;

namespace MVCFoodProject.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public ApplicationDbContext _db;

        public UserController(ApplicationDbContext context)
        {
            _db = context;
        }

        [HttpPost("/user/edit")]
        public async Task<ActionResult> EditUserProfile([FromForm] EditProfileDTO body)
        {
            var user = (Users)HttpContext.Items["User"]!;

            try
            {
                if (body.image != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await body.image.CopyToAsync(memoryStream);
                        var imageBytes = memoryStream.ToArray();
                        var base64String = Convert.ToBase64String(imageBytes);

                        user.imgURL = $"data:{body.image.ContentType};base64, {base64String}";
                    }
                }

                if (body.Address != null)
                {
                    user.Adress = body.Address;
                }

                if (body.Name != null)
                {
                    user.Name = body.Name;
                }

                if (body.Number != null)
                {
                    user.Number = body.Number;
                }

                if (_db.ChangeTracker.HasChanges())
                {
                    await _db.SaveChangesAsync();
                }
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(user);
            
        }
    }
}
