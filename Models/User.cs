using System.ComponentModel.DataAnnotations;

namespace OfflineTicketingSystem.Models;

public enum Role { Employee, Admin }

public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string FullName { get; set; } = null!;

    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public byte[] PasswordHash { get; set; } = null!;

    [Required]
    public byte[] PasswordSalt { get; set; } = null!;

    [Required]
    public Role Role { get; set; }

    public List<Ticket> CreatedTickets { get; set; } = new();
    public List<Ticket> AssignedTickets { get; set; } = new();
}
