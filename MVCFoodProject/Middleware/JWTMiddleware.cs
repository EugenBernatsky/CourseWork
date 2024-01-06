using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;

namespace MVCFoodProject.Middleware
{
    public class JWTMiddleware
    {
        private readonly RequestDelegate _next;

        public JWTMiddleware(
            RequestDelegate next
           )
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ApplicationDbContext _db, UserManager<IdentityUser> _userManager)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var cookieToken = context.Request.Cookies["identity"];

            try
            {
                if (token != null || cookieToken != null)
                {
                    JwtSecurityToken decodedToken = new JwtSecurityToken(token ?? cookieToken);

                    var email = decodedToken.Claims
                                .Where(c => c.Type == ClaimTypes.Email)
                                .Select(c => c.Value)
                                .SingleOrDefault();

                    var currentUser = await _userManager.FindByEmailAsync(email);

                    if (currentUser != null)
                    {
                        context.Items["User"] = _db.User.Where(u => u.UID == currentUser.Id).FirstOrDefault();
                    }


                }
            } catch ( Exception ex )
            {
                Console.WriteLine(ex.ToString());
            }

            

            await _next(context);
        }
    }
}
