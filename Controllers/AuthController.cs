using BusinessManagement.Api.DTOs;
using BusinessManagement.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService authService)
    {
        _auth = authService;
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var result = await _auth.RegisterAsync(request);
            return CreatedAtAction(nameof(Register), new { id = result.UserId }, result);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginDto loginData)
    {
        // Check if model is valid
        if (!ModelState.IsValid)
        {
            var errorList = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            
            return BadRequest(new { 
                message = "Bad request data", 
                errors = errorList 
            });
        }

        // Null check
        if (loginData == null)
        {
            return BadRequest(new { message = "Need request body" });
        }

        // Check email and password
        if (string.IsNullOrWhiteSpace(loginData.Email) || string.IsNullOrWhiteSpace(loginData.Password))
        {
            return BadRequest(new { message = "Need email and password" });
        }

        try
        {
            // Make login request for service
            var loginReq = new LoginRequest
            {
                Email = loginData.Email.Trim().ToLower(),
                Password = loginData.Password
            };

            var authResult = await _auth.LoginAsync(loginReq);
            
            // Return token
            return Ok(new { token = authResult.Token });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { message = "Wrong login" });
        }
        catch (KeyNotFoundException)
        {
            return Unauthorized(new { message = "Wrong login" });
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
        {
            Console.WriteLine("DB error: " + ex.Message);
            return StatusCode(500, new { message = "Server error" });
        }
        catch (Npgsql.NpgsqlException ex)
        {
            Console.WriteLine("Postgres error: " + ex.Message);
            return StatusCode(500, new { message = "Server error" });
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine("Service error: " + ex.ToString()); // FIX: Show full error
            return StatusCode(500, new { message = "Server error" });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Login error: " + ex.ToString()); // FIX: Show full error
            return StatusCode(500, new { message = "Server error" });
        }
    }
}