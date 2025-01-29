using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoteManagementSystem.Models;
using NoteManagementSystem.Services;
using NoteManagementSystem.Auth;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NoteManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly JwtService _jwtService;
        private static readonly HashSet<string> _blacklistedTokens = new HashSet<string>(); // Store invalid tokens

        public UserController(UserService userService, JwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                return BadRequest(new { Message = "Email and password are required." });
            }

            user.Email = user.Email.ToLower();

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

            var token = _jwtService.GenerateToken(user);
            return Ok(new { Token = token });
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new { Message = "No token provided." });
            }

            JwtService.BlacklistedTokens.Add(token); 

            return Ok(new { Message = "Logged out successfully." });
        }

        [HttpGet("isTokenValid")]
        public IActionResult IsTokenValid()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (_blacklistedTokens.Contains(token))
            {
                return Unauthorized(new { Message = "Token has been revoked. Please log in again." });
            }
            return Ok(new { Message = "Token is valid." });
        }
    }

    public class LoginRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
