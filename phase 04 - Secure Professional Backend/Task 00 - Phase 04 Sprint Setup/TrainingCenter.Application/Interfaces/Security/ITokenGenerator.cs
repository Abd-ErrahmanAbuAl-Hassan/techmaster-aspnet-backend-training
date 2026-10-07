using TrainingCenter.Application.Helpers.Models;
using TrainingCenter.Domain.Entities;

namespace TrainingCenter.Application.Interfaces.Security
{
    public interface ITokenGenerator
    {
        string GenerateJwtToken(User user, Jwt _jwt);
        string GenerateRefreshToken();
        string HashToken(string token);
    }
}
