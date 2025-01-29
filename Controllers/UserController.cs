using Microsoft.AspNetCore.Mvc;
using NoteManagementSystem.Models;
using NoteManagementSystem.Services;
using System.Threading.Tasks;
using NoteManagementSystem.Auth;

namespace NoteManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly JwtService _jwtService;

        public UserController(UserService userService, JwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

    [HttpPost("signup")]
    public async Task<IActionResult> Register([FromBody] User user)
    {
        user.Email = user.Email.ToLower(); 

        if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            return BadRequest(new { Message = "Email and password are required." });
        }

        var existingUser = await _userService.GetByEmailAsync(user.Email);
        if (existingUser != null)
        {
            return BadRequest(new { Message = "User with this email already exists." });
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

        await _userService.CreateAsync(user); 

        return Ok(new { Message = "User registered successfully." });
    }



        [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        if (string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Password))
        {
            return BadRequest(new { Message = "Email and password are required." });
        }

        var isValid = await _userService.ValidateCredentialsAsync(loginRequest.Email, loginRequest.Password);
        if (!isValid)
        {
            return Unauthorized(new { Message = "Invalid email or password." });
        }

        var user = await _userService.GetByEmailAsync(loginRequest.Email);
        if (user == null)
        {
            return Unauthorized(new { Message = "Invalid email or password." });
        }

        if (string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            return StatusCode(500, new { Message = "User password hash is missing." });
        }

        var token = _jwtService.GenerateToken(user);
        return Ok(new { Token = token });
    }

    }

    public class LoginRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
