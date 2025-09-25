using minimal_api.Domain.Entities;

namespace minimal_api.Infrastructure.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(Administrator admin);
    }
}
