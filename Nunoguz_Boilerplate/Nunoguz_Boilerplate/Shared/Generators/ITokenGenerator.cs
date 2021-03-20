using Nunoguz_Boilerplate.Domain.Models;

namespace Nunoguz_Boilerplate.Shared.Generators
{
    public interface ITokenGenerator
    {
        string GenerateToken(User user);
    }
}
