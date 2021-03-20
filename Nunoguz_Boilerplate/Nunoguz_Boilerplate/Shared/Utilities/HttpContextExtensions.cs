using Nunoguz_Boilerplate.Domain.Models;
using Nunoguz_Boilerplate.Persistence.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace Nunoguz_Boilerplate.Shared.Utilities
{
    public static class HttpContextExtensions
    {
        public static User GetThisUser(this HttpContext context, DatabaseContext _dbContext)
        {
            if (context.User != null && context.User.Claims.Count() > 0)
            {
                var claims = context.User.Claims.ToDictionary(u => u.Type, u => u.Value);
                string userId = claims[ClaimTypes.NameIdentifier].ToString();

                return _dbContext.Users.Where(s => s.Id == int.Parse(userId) && s.isActive).FirstOrDefault();
            }
            return null;
        }
    }
}
