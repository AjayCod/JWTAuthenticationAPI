using Application.Contracts;
using Application.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuthenticationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class User : ControllerBase
    {
        private readonly IUser user;
        public User(IUser user)
        {
            this.user = user;
        }
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> LoginUserIn(LoginDTO loginDTO)
        {
            var result = await user.LoginUserAsync(loginDTO);
            return Ok(result);
        }
        [HttpPost("register")]
        public async Task<ActionResult<LoginResponse>> RegisterUser(RigesterUserDTO userDTO)
        {
            var result = await user.RegistrationUserAsync(userDTO);
            return Ok(result);
        }
    }
}
