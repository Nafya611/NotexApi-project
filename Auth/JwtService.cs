using Microsoft.IdentityModel.Tokens;
using NoteManagementSystem.Config;
using NoteManagementSystem.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NoteManagementSystem.Auth
{
    public class JwtService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtService(JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        public string GenerateToken(User user)
    {
        if (string.IsNullOrEmpty(_jwtSettings.Secret))
            throw new InvalidOperationException("JWT Secret cannot be null or empty.");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        if (user.Id == null || user.Email == null)
            throw new ArgumentException("User ID and Email cannot be null.");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtSettings.ExpirationInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    }
}
