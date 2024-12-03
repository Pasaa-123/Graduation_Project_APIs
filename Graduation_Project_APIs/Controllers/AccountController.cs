using Graduation_Project_APIs.Data;
using Graduation_Project_APIs.DTO;
using Graduation_Project_APIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Graduation_Project_APIs.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration config;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, IConfiguration config, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.config = config;
            this.signInManager = signInManager;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(ApplicationUserDTO userDTO)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser()
                {
                    Email = userDTO.Email,
                    UserName = userDTO.UserName,
                };
                var success = await userManager.CreateAsync(user, userDTO.Password);
                if (success.Succeeded)
                {

                    return Ok($"User Created");
                }
                else
                {
                    foreach (var item in success.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO UserFromRequest)
        {
            if (ModelState.IsValid)
            {
                var User = await userManager.FindByNameAsync(UserFromRequest.UserName);
                if (User != null)
                {
                    var found = await userManager.CheckPasswordAsync(User, UserFromRequest.Password);
                    if (found)
                    {
                        var UserClaims = new List<Claim>();
                        UserClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                        UserClaims.Add(new Claim(ClaimTypes.NameIdentifier, User.Id));
                        UserClaims.Add(new Claim(ClaimTypes.Name, User.UserName));
                        var UserRoles = await userManager.GetRolesAsync(User);
                        foreach (var Role in UserRoles)
                        {
                            UserClaims.Add(new Claim(ClaimTypes.Role, Role));
                        }
                        var SignInKey =
                           new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                               config["JWT:SecritKey"]));

                        SigningCredentials signingCred =
                            new SigningCredentials
                            (SignInKey, SecurityAlgorithms.HmacSha256);
                        //design our token
                        JwtSecurityToken token = new JwtSecurityToken(
                            issuer: config["IssuerIP"],
                            audience: config["AudienceIP"],
                            expires: DateTime.Now.AddDays(14),
                            claims: UserClaims,
                            signingCredentials: signingCred
                            );
                        //generate token
                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = DateTime.Now.AddDays(14)
                        });
                    }
                    ModelState.AddModelError("Password", "Invalid Password");
                }

                ModelState.AddModelError("UserName", "Invalid UserName");

            }
            return BadRequest(ModelState);
        }
        [HttpDelete("Logout")]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Ok("User is signed out");
        }
    }
}