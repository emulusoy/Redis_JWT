using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Redis_JWT.Application.Features.Auth.Commands.RegisterUser;
using Redis_JWT.Application.Features.Auth.Queries.Login;

namespace Redis_JWT.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IMediator mediator) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginQuery query)
        {           
            var result = await mediator.Send(query);
            return result.IsSuccess
                ? Ok(new { message = "Login Successful - Giris Basarili?", result.Value })         //eger bos donmuyorsa ? demektir mesaj ve value yani benim olusan Keyi mi donderir!
                : Unauthorized(new { error = "Login Failed - Giris Basarisiz!" });               //hata icin error mesaji verdim ayrica Auth cikisi verdim
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserCommand command)
        {
            var result = await mediator.Send(command);
            return result.IsSuccess
                ? Ok(new {message="Registration Successful - Kayit Basarili!"})                                          
                : BadRequest(new { error = "Registration Failed - Kayit Basarisiz" }); 
        }
        [HttpGet("AuthOrNo")]
        [AllowAnonymous] // Global Authorize header varsa kimlik doğrulaması yapılır
        public IActionResult AuthOrNo()
        {
            var isAuth = User?.Identity?.IsAuthenticated == true;
            var claims = isAuth
                ? User!.Claims.Select(c => new { c.Type, c.Value })
                : Enumerable.Empty<object>();

            return Ok(new { isAuthenticated = isAuth, claims });
        }
    }
}
