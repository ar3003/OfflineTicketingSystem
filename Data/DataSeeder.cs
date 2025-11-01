using System.Security.Cryptography;
using OfflineTicketingSystem.Models;

namespace OfflineTicketingSystem.Data;

public static class DataSeeder
{
    public static void Seed(AppDbContext ctx)
    {
        ctx.Database.EnsureCreated();

        if (ctx.Users.Any()) return; // already seeded

        // helper to create password hash/salt
        static (byte[] hash, byte[] salt) CreateHash(string password)
        {
            using var hmac = new HMACSHA512();
            var salt = hmac.Key;
            var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return (hash, salt);
        }

        var (adminHash, adminSalt) = CreateHash("Admin@123");
        var (empHash, empSalt) = CreateHash("Employee@123");

        var admin = new User
        {
            FullName = "Admin User",
            Email = "admin@example.com",
            PasswordHash = adminHash,
            PasswordSalt = adminSalt,
            Role = Role.Admin
        };

        var employee = new User
        {
            FullName = "Employee User",
            Email = "employee@example.com",
            PasswordHash = empHash,
            PasswordSalt = empSalt,
            Role = Role.Employee
        };

        ctx.Users.AddRange(admin, employee);
        ctx.SaveChanges();

        // sample ticket
        var ticket = new Ticket
        {
            Title = "Cannot access printer",
            Description = "The Network printer doesn't print.",
            Priority = TicketPriority.Medium,
            Status = TicketStatus.Open,
            CreatedByUserId = employee.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        ctx.Tickets.Add(ticket);
        ctx.SaveChanges();
    }
}
