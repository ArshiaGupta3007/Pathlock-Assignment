using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProjectManagerApi.Data;
using ProjectManagerApi.DTOs;
using ProjectManagerApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectManagerApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher<User> _pwdHasher;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext db, IPasswordHasher<User> pwdHasher, IConfiguration config)
        {
            _db = db;
            _pwdHasher = pwdHasher;
            _config = config;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            if (await _db.Users.AnyAsync(u => u.Username == dto.Username))
                throw new ApplicationException("Username already exists.");

            var user = new User { Username = dto.Username };
            user.PasswordHash = _pwdHasher.HashPassword(user, dto.Password);
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var token = GenerateJwt(user);
            return new AuthResponseDto { Token = token };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username)
                       ?? throw new ApplicationException("Invalid credentials.");

            var res = _pwdHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (res == PasswordVerificationResult.Failed)
                throw new ApplicationException("Invalid credentials.");

            var token = GenerateJwt(user);
            return new AuthResponseDto { Token = token };
        }

        private string GenerateJwt(User user)
        {
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim("id", user.Id.ToString())
            };

            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddDays(7);
            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: expires,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
