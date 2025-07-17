// SeuProjeto.Api/Controllers/AuthController.cs
using Application.Interfaces;
using Domain.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Queries;
using Microsoft.AspNetCore.Authorization;
using Domain.Entities;
using Application.Commands;

namespace SeuProjeto.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService, IMediator mediator)
        {
            _authService = authService;
            _mediator = mediator;
        }

        [HttpPost("register")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Register(RegisterRequestDto dto)
        {
            var (success, errors) = await _authService.RegisterAsync(dto);

            if (!success) return BadRequest(new { Errors = errors });

            return Ok(new { Message = "Usuário registrado com sucesso!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto dto)
        {
            var (success, token, roles) = await _authService.LoginAsync(dto);

            if (!success) return Unauthorized(new { Message = "Usuário ou senha inválidos" });

            return Ok(new
            {
                Token = token,
                Roles = roles
            });
        }

        [HttpGet("users")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> ListUsers()
        {
            var users = await _mediator.Send(new ListUsersQuery());
            return Ok(users);
        }

        [HttpGet("roles")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> ListRoles()
        {
            var roles = await _mediator.Send(new ListRolesQuery());
            return Ok(roles);
        }

        [HttpPut("users/{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserCommand command)
        {
            if (id != command.Id)
                return BadRequest("Id da URL difere do corpo da requisição.");
            var result = await _mediator.Send(command);
            if (!result)
                return NotFound("Usuário não encontrado ou erro ao atualizar.");
            return Ok("Usuário atualizado com sucesso.");
        }

        [HttpDelete("users/{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _mediator.Send(new DeleteUserCommand { Id = id });
            if (!result)
                return NotFound("Usuário não encontrado ou erro ao deletar.");
            return Ok("Usuário deletado com sucesso.");
        }
    }
}