using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVCFoodProject.Models.DTO;
using System.Data;

namespace MVCFoodProject.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "courier")]
    [Route("api/[controller]")]
    [ApiController]

    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
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

            if (user == null)
            {
                return BadRequest();
            }

            if(body.Name != null)
            {
                user.Name = body.Name;
            }

            if(body.Adress != null) 
            {
                user.Adress = body.Adress;
            }

            if(body.Number != null)
            {
                user.Number = body.Number;
            }

            if(body.imgURL != null)
            {
                user.imgURL = body.imgURL;
            }

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
