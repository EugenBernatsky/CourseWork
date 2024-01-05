using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVCFoodProject.Models.DTO;
using System.Data;

namespace MVCFoodProject.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "customer")]
    [Route("api/[controller]")]
    [ApiController]

    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public CustomerController(ApplicationDbContext context)
        {
            _db = context;
        }

        [HttpPost("/customer/edit")]
        public async Task<ActionResult<Users>> EditProfile([FromBody] EditProfileDTO body)
        {
            var contextUser = (Users)HttpContext.Items["User"];

            var user = await _db.User
                .Where(u => u.Id == contextUser.Id)
                .FirstOrDefaultAsync();

            user.Name = body.Name ?? user.Name;
            user.Adress = body.Adress ?? user.Adress;
            user.Number = body.Number ?? user.Number;
            user.imgURL = body.imgURL ?? user.imgURL;

            try
            {
                if(_db.ChangeTracker.HasChanges())
                {
                    await _db.SaveChangesAsync();
                }
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(user);
        }
    }
}
