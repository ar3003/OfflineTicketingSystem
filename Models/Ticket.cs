using System.ComponentModel.DataAnnotations;

namespace OfflineTicketingSystem.Models;

public enum TicketStatus { Open, InProgress, Closed }
public enum TicketPriority { Low, Medium, High }

public class Ticket
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    [Required]
    public TicketStatus Status { get; set; } = TicketStatus.Open;

    [Required]
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public Guid CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }

    public Guid? AssignedToUserId { get; set; }
    public User? AssignedToUser { get; set; }
}
