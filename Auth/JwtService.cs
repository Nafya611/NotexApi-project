using Microsoft.IdentityModel.Tokens;
using NoteManagementSystem.Config;
using NoteManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NoteManagementSystem.Auth
{
    public class JwtService
    {
        private readonly JwtSettings _jwtSettings;
        public static readonly HashSet<string> BlacklistedTokens = new HashSet<string>(); 

        public JwtService(JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        public string GenerateToken(User user)
        {
            if (string.IsNullOrEmpty(_jwtSettings.Secret))
                throw new InvalidOperationException("JWT Secret cannot be null or empty.");
            
            if (string.IsNullOrEmpty(user.Id) || string.IsNullOrEmpty(user.Email))
                throw new ArgumentException("User ID and Email cannot be null or empty.");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

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
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool IsTokenValid(string token)
        {
            return !BlacklistedTokens.Contains(token); 
        }

        public void InvalidateToken(string token)
        {
            BlacklistedTokens.Add(token); 
        }
    }
}
