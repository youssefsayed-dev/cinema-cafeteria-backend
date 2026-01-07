using BusinessManagement.Api.Data;
using BusinessManagement.Api.DTOs;
using BusinessManagement.Api.Entities;
using BusinessManagement.Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BusinessManagement.Api.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _db = context;
        _config = configuration;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest req)
    {
        // Check if user exists
        if (await _db.Users.AnyAsync(u => u.Email == req.Email))
        {
            throw new InvalidOperationException("User already exists");
        }

        // Check role
        if (!Enum.TryParse<UserRole>(req.Role, true, out var role))
        {
            throw new ArgumentException("Bad role");
        }

        // Hash password
        var hash = BCrypt.Net.BCrypt.HashPassword(req.Password);

        // Create user
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = req.Email,
            PasswordHash = hash,
            Role = role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // Make token
        var token = MakeToken(user);

        return new AuthResponse
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest req)
    {
        // Check input
        if (req == null)
        {
            throw new ArgumentNullException(nameof(req), "Request is null");
        }

        if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
        {
            throw new ArgumentException("Need email and password");
        }

        // Normalize email
        var emailLower = req.Email.Trim().ToLower();

        // Get user
        User? user = null;
        try
        {
            user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == emailLower);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("DB error", ex);
        }

        // Check user
        if (user == null)
        {
            throw new UnauthorizedAccessException("Wrong login");
        }

        // Check password hash
        if (string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            throw new InvalidOperationException("User setup wrong");
        }

        // Verify password
        bool passwordOk = false;
        try
        {
            passwordOk = BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Password check failed", ex);
        }

        if (!passwordOk)
        {
            throw new UnauthorizedAccessException("Wrong login");
        }

        // Check if active
        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("User not active");
        }

        // Make token
        string token;
        try
        {
            token = MakeToken(user);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Token error: " + ex.Message);
            throw new InvalidOperationException("Token failed", ex);
        }

        return new AuthResponse
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }

    private string MakeToken(User user)
    {
        var jwtSettings = _config.GetSection("JwtSettings");
        if (jwtSettings == null)
        {
            throw new InvalidOperationException("No JWT settings");
        }

        var keyStr = jwtSettings["Key"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        if (string.IsNullOrWhiteSpace(keyStr))
        {
            throw new InvalidOperationException("No JWT key");
        }

        if (string.IsNullOrWhiteSpace(issuer))
        {
            throw new InvalidOperationException("No issuer");
        }

        if (string.IsNullOrWhiteSpace(audience))
        {
            throw new InvalidOperationException("No audience");
        }

        try
        {
            var key = Encoding.UTF8.GetBytes(keyStr);

            // Check key length
            if (key.Length < 16)
            {
                throw new InvalidOperationException("Key too short");
            }

            Console.WriteLine($"Making token for user: {user.Email}");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var creds = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Token make error: " + ex.Message);
            throw new InvalidOperationException("Token creation failed", ex);
        }
    }
}