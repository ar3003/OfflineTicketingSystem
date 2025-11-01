using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfflineTicketingSystem.Data;
using OfflineTicketingSystem.DTOs;
using OfflineTicketingSystem.Models;
using System.Security.Claims;

namespace OfflineTicketingSystem.Controllers;

[ApiController]
[Route("tickets")]
public class TicketsController : ControllerBase
{
    private readonly AppDbContext _ctx;

    public TicketsController(AppDbContext ctx)
    {
        _ctx = ctx;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> Create([FromBody] CreateTicketDto dto)
    {
        var ticket = new Ticket
        {
            Title = dto.Title,
            Description = dto.Description,
            Priority = Enum.Parse<TicketPriority>(dto.Priority, true),
            Status = TicketStatus.Open,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedByUserId = CurrentUserId
        };
        _ctx.Tickets.Add(ticket);
        await _ctx.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
    }

    [HttpGet("my")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> MyTickets()
    {
        var tickets = await _ctx.Tickets
            .Where(t => t.CreatedByUserId == CurrentUserId)
            .ToListAsync();
        return Ok(tickets);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> All()
    {
        var tickets = await _ctx.Tickets.Include(t => t.CreatedByUser).Include(t => t.AssignedToUser).ToListAsync();
        return Ok(tickets);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id)
    {
        var ticket = await _ctx.Tickets.Include(t => t.CreatedByUser).Include(t => t.AssignedToUser).FirstOrDefaultAsync(t => t.Id == id);
        if (ticket == null) return NotFound();

        var userId = CurrentUserId;
        var isCreator = ticket.CreatedByUserId == userId;
        var isAssignedAdmin = ticket.AssignedToUserId == userId && User.IsInRole("Admin");
        if (isCreator || isAssignedAdmin || User.IsInRole("Admin"))
            return Ok(ticket);

        return Forbid();
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTicketDto dto)
    {
        var ticket = await _ctx.Tickets.FindAsync(id);
        if (ticket == null) return NotFound();

        if (!string.IsNullOrEmpty(dto.Status))
            ticket.Status = Enum.Parse<TicketStatus>(dto.Status, true);
        if (!string.IsNullOrEmpty(dto.Priority))
            ticket.Priority = Enum.Parse<TicketPriority>(dto.Priority, true);
        if (dto.AssignedToUserId.HasValue)
        {
            var admin = await _ctx.Users.FindAsync(dto.AssignedToUserId.Value);
            if (admin == null || admin.Role != Role.Admin) return BadRequest(new { message = "Assigned user must be an existing Admin" });
            ticket.AssignedToUserId = dto.AssignedToUserId;
        }

        ticket.UpdatedAt = DateTime.UtcNow;
        await _ctx.SaveChangesAsync();
        return Ok(ticket);
    }

    [HttpGet("stats")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Stats()
    {
        var counts = await _ctx.Tickets
            .GroupBy(t => t.Status)
            .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
            .ToListAsync();
        return Ok(counts);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var ticket = await _ctx.Tickets.FindAsync(id);
        if (ticket == null) return NotFound();
        _ctx.Tickets.Remove(ticket);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }
}
