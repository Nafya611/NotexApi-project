using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly UserService _userService;

    public AuthController(IConfiguration config, UserService userService)
    {
        _config = config;
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Fetch user from MongoDB
        var user = await _userService.GetUserByUsernameAsync(request.Username);

        // Check if user exists and if passwords match
        if (user == null || user.Password != request.Password)
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        // Generate JWT token
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtKey = _config["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "JWT key is not configured properly." });
        }

        var key = Encoding.UTF8.GetBytes(jwtKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
            }),
            Expires = DateTime.UtcNow.AddHours(2), // Set the token expiration time
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        // Return the JWT token and expiration time
        return Ok(new
        {
            Token = tokenHandler.WriteToken(token),
            Expires = tokenDescriptor.Expires
        });
    }
}

public class LoginRequest
{
    public required string Username { get; set; } // Required username from request
    public required string Password { get; set; } // Required password from request
}
