using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserWebApi.User;

namespace UserWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;


        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost("Register")]     
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var response = new Response();
            try
            {
                var userExists = await userManager.FindByNameAsync(model.Username);
                if (userExists != null)
                {
                    response.Code = 500;
                    response.Status = "Error";
                    response.Message = "User already exists!";
                }
                else
                {
                    ApplicationUser user = new ApplicationUser()
                    {
                        Email = model.Email,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        UserName = model.Username,
                        Activo = true,
                    };

                    var result = await userManager.CreateAsync(user, model.Password);
                    if (!result.Succeeded)
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });
                    if (!await roleManager.RoleExistsAsync(UserRoles.User))
                        await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
                    await userManager.AddToRoleAsync(user, UserRoles.User);
                    response.Code = 200;
                    response.Status = "Success";
                    response.Message = "User created successfully!";

                }
                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Status = "Error";
                response.Message = ex.Message;
                return new JsonResult(response);
            }
            finally {
               //Maybe do log here in a future
            }
           
        }

        [HttpPost("OnlyAdmin")]
        [Authorize(Policy = "UserRestrict")]
        public async Task<IActionResult> OnlyAdmin([FromBody] RegisterModel model)
        {
            return  new JsonResult("asdasd");
        }
    }
}
