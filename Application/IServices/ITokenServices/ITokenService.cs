using Domain.Entities.ApplicationEntities;

namespace Application.IServices.ITokenServices;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(ApplicationUser user);
}