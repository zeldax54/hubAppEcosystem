using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using UserWebApi.User;
using DTO;
using System.Net.Http;
using System;
using System.Net;
using Flurl.Http;
using System.Text;
using Newtonsoft.Json;
using IdentityModel;
using IdentityModel.Client;

namespace Midgard.Controllers
{
    [ApiController]
    [Route("midgard/[controller]")]
    public class UserController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly ITokenProviders _tokenProvider;


        public UserController(IConfiguration configuration, ITokenProviders tokenProvider)
        {          
            _configuration = configuration;
            _tokenProvider = tokenProvider;
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var tokenResponse = await _tokenProvider.GetUserToken(model.Username,model.Password);
            return new JsonResult(tokenResponse);
        }

       [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {        
            var registerUserURL = _configuration.GetSection("UserApiRegister").Get<string>();
            try
            {              
                var tokenResponse = await _tokenProvider.GetAPIToken();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(registerUserURL),
                    Content = content
                };
                var flurClient = new FlurlClient();
                flurClient.WithHeader("Content-Type", "application/json");
                flurClient.HttpClient.SetBearerToken(tokenResponse.Token);
                var result =  flurClient.HttpClient.SendAsync(request)
                    .ConfigureAwait(false).GetAwaiter().GetResult().Content.ReadAsStringAsync();
                return new JsonResult(result);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response { Status = "Error", Message = ex.Message });
            }         
        }

        
    }
}
