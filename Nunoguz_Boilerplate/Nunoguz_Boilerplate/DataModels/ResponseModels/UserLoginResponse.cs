using Nunoguz_Boilerplate.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nunoguz_Boilerplate.DataModels.ResponseModels
{
    public class UserLoginResponse : BaseModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
        public string Token { get; set; }

        public UserLoginResponse(User user, string token)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Image = user.ImageUrl;
            Token = token;
        }

        public UserLoginResponse(User user)
        {
            Id = user.Id;
            Email = user.Email;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Image = user.ImageUrl;
        }
    }
}
