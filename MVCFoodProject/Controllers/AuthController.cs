using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MVCFoodProject.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;

        public AuthController (
            IConfiguration config,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context
         )
        {
            _config = config;
            _signInManager = signInManager;
            _userManager = userManager;
            _db = context;
        }

        [AllowAnonymous]
        [HttpPost("/login")]
        public IActionResult Login([FromBody] LoginModel login)
        {

            var user = Authenticate(login.Username, login.Password);
             
            if (user.Result != null)
            {
                var internalUser = _db.User.Where(p => p.UID == user.Result.Id).First() ;

                if (internalUser != null)
                {
                    var token = GenerateToken(login.Username, getUserRole(internalUser.Role));


                    return Ok(new UserProfile
                    {
                        token = token,
                        role = getUserRole(internalUser.Role),
                        userId = user.Result.Id
                    });
                } else
                {
                    return BadRequest("Login Failed");
                }
               
            }

            return BadRequest("Login Failed");
        }

        [AllowAnonymous]
        [HttpPost("/register")]
        public async Task<ActionResult> Register ([FromBody] RegisterModel body )
        {
            IdentityUser user = new IdentityUser { 
                UserName = body.username,
                PhoneNumber = body.phone,
                Email = body.email
            };

            var result = await _userManager.CreateAsync(user, body.password);

            if (result.Succeeded)
            {
                _db.User.Add(new Users {
                    Name = user.UserName,
                    UID = user.Id,
                    Number = user.PhoneNumber
                });

                await _db.SaveChangesAsync();

                var token = GenerateToken(body.username, getUserRole(Users.UserRole.Customer));

                return Ok(new UserProfile
                {
                    token = token,
                    role = getUserRole(Users.UserRole.Customer),
                    userId = user.Id
                });
            }
            else
            {
                return BadRequest(result);
            }

        }

        private async Task<IdentityUser?> Authenticate(string UserName, string Password)
        {
            var result = await _signInManager.PasswordSignInAsync(UserName, Password, false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var currentUser = await _userManager.FindByNameAsync(UserName);

                if (currentUser != null)
                {
                    return currentUser;
                }
            }

            return null;
        }


        private string GenerateToken (string UserName, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("JWT:Key").Value));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, UserName),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                _config.GetSection("JWT:Issuer").Value, _config.GetSection("JWT:Audience").Value,
                claims,
                expires: DateTime.Now.AddDays(10),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string getUserRole (Users.UserRole role)
        {
            switch (role)
            {
                case Users.UserRole.Admin:
                    return "admin";
                case Users.UserRole.Customer:
                    return "customer";
                case Users.UserRole.Courier:
                    return "courier";
                default:
                    return "unknown";
            }
        }
    }
}
