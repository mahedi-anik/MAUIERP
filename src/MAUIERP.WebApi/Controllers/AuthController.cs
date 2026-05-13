using MAUIERP.ApplicationLayer.Common.Models;
using MAUIERP.ApplicationLayer.DTOs.Auth;
using MAUIERP.ApplicationLayer.Features.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MAUIERP.WebApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator) => _mediator = mediator;

        [HttpPost("login")]
        public async Task<ActionResult<Result<LoginResponseDto>>> Login(LoginCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }
    }
}
