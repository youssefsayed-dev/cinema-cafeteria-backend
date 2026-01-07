using BusinessManagement.Api.Data;
using BusinessManagement.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace BusinessManagement.Api.Services;

public class DatabaseSeeder
{
    private readonly AppDbContext _db;

    public DatabaseSeeder(AppDbContext context)
    {
        _db = context;
    }

    public async Task SeedAdminUserAsync() 
    {
        try
        {
            Console.WriteLine("Starting seed...");

            // Check connection
            if (!await _db.Database.CanConnectAsync())
            {
                Console.WriteLine("Cannot connect to DB");
                return;
            }

            // Do migrations
            try
            {
                var pending = await _db.Database.GetPendingMigrationsAsync();
                if (pending.Any())
                {
                    Console.WriteLine("Doing migrations: " + pending.Count());
                    await _db.Database.MigrateAsync();
                    Console.WriteLine("Migrations done");
                }
                else
                {
                    Console.WriteLine("No migrations needed");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Migration error: " + ex.Message);
            }

            // Make ShiftId nullable if needed
            try
            {
                await _db.Database.ExecuteSqlRawAsync("ALTER TABLE \"Sales\" ALTER COLUMN \"ShiftId\" DROP NOT NULL;");
                Console.WriteLine("Made ShiftId nullable");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Alter table error: " + ex.Message);
            }

            // Check if admin exists
            var adminEmail = "admin@cinema.com";
            var emailLower = adminEmail.ToLower();

            var adminExists = await _db.Users
                .AnyAsync(u => u.Email.ToLower() == emailLower);

            if (adminExists)
            {
                Console.WriteLine("Admin already exists");
                return;
            }

            // Create admin
            var adminPass = "Admin123!";
            var hash = BCrypt.Net.BCrypt.HashPassword(adminPass);

            var adminUser = new User
            {
                Id = new Guid("11111111-1111-1111-1111-111111111111"),
                Email = adminEmail,
                PasswordHash = hash,
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _db.Users.Add(adminUser);
            await _db.SaveChangesAsync();

            Console.WriteLine("Admin user created");
            Console.WriteLine("Login: admin@cinema.com / Admin123!");
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
        {
            Console.WriteLine("DB error: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}