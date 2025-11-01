namespace OfflineTicketingSystem.DTOs;

public class UpdateTicketDto
{
    public string? Status { get; set; } // Open, InProgress, Closed
    public string? Priority { get; set; } // Low, Medium, High
    public Guid? AssignedToUserId { get; set; }
}
