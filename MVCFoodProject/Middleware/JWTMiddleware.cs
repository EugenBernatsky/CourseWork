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

            if (token != null || cookieToken != null)
            {
                JwtSecurityToken decodedToken = new JwtSecurityToken(token ?? cookieToken);

                var name = decodedToken.Claims
                            .Where(c => c.Type == ClaimTypes.NameIdentifier)
                            .Select(c => c.Value)
                            .SingleOrDefault();

                var currentUser = await _userManager.FindByNameAsync(name);
                    if (currentUser != null)
                    {
                        context.Items["User"] = _db.User.Where(u => u.UID == currentUser.Id).FirstOrDefault();
                    }
                
                
            }

            await _next(context);
        }
    }
}
