using AnonymousMessageApplication.Data;
using AnonymousMessageApplication.DTOs;
using AnonymousMessageApplication.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AnonymousMessageApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // SUPERADMIN registers new Admins
        [HttpPost("register-admin")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminRequest request)
        {
            if (await _context.Admins.AnyAsync(a => a.Username == request.Username))
                return BadRequest("Username already exists");

            var admin = new Admin
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "Admin"
            };

            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Admin registered successfully", AdminId = admin.Id });
        }

        // Login (works for both SuperAdmin & Admins)
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Username == request.Username);
            if (admin == null || !BCrypt.Net.BCrypt.Verify(request.Password, admin.PasswordHash))
                return Unauthorized("Invalid credentials");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, admin.Username),
                new Claim(ClaimTypes.Role, admin.Role),
                new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:ExpireMinutes"])),
                signingCredentials: creds
            );

            return Ok(new AuthResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Username = admin.Username,
                UserId = admin.Id
            });
        }

        // Get all admins (Only accessible by SuperAdmin)
        [HttpGet("admins")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetAllAdmins()
        {
            var admins = await _context.Admins
                .Select(a => new
                {
                    a.Id,
                    a.Username,
                    a.Role
                })
                .ToListAsync();

            return Ok(admins);
        }

    }
}
