using OfflineTicketingSystem.Models;

namespace OfflineTicketingSystem.Services;

public interface ITokenService
{
    string CreateToken(User user);
}
