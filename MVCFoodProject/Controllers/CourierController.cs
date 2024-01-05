using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace MVCFoodProject.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "courier")]
    [Route("api/[controller]")]
    [ApiController]

    public class CourierController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public CourierController(ApplicationDbContext context)
        {
            _db = context;
        }

        [HttpPost("/courier/edit")]
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

            user.Name = body.Name ?? user.Name;
            user.Adress = body.Adress ?? user.Adress;
            user.Number = body.Number ?? user.Number;
            user.imgURL = body.imgURL ?? user.imgURL;

            try
            {
                if (_db.ChangeTracker.HasChanges())
                {
                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(user);
        }

        [HttpPost("/orders/toggle")]
        public async Task<ActionResult<Orders>> ToggleOrders([FromBody] ToggleOrderDTO body)
        {
            var contextUser = (Users)HttpContext.Items["User"];

            var order = await _db.Order
                .Where(o => o.Id == body.Id)
                .FirstOrDefaultAsync();

            var courier = await _db.Courier
                .Where(c => c.User.Id == contextUser.Id)
                .FirstOrDefaultAsync();
            try
            {
                order.status = body.action == ToggleOrderDTO.ACTION.take ? Orders.Status.Taken : Orders.Status.Canceled;
                courier.status = body.action == ToggleOrderDTO.ACTION.take ? Courier.Status.busy : Courier.Status.free;

                await _db.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

    }
}
