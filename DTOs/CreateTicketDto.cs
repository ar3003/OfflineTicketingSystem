namespace OfflineTicketingSystem.DTOs;

public class CreateTicketDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string Priority { get; set; } = "Medium"; // Low, Medium, High
}
