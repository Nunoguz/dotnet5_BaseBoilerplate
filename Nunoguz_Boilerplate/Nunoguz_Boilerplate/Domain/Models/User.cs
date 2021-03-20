using Nunoguz_Boilerplate.DataModels.RequestModels.Users;
using Nunoguz_Boilerplate.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Nunoguz_Boilerplate.Domain.Models
{
    public class User : BaseModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
        public string GsmNumber { get; set; }
        public string ImageUrl { get; set; }
        [JsonIgnore]
        public string HashedPassword { get; set; }
        [JsonIgnore]
        public string Salt { get; set; }
        public string City { get; set; }
        public float? Height { get; set; }
        public int? Weight { get; set; }
        public DateTime? DateofBirth { get; set; }
        public EGender? Gender { get; set; } = EGender.unspecified;
        //public EMembershipType? MembershipType { get; set; } = 0;
        //public EPrivacy? Privacy { get; set; }
        //public List<Code> Codes { get; set; }
        public bool EmailConfirmed { get; set; } = false;

        public User() { }
        
        public User(CreateUserRequest request, Tuple<string, string> hashTuple)
        {
            Email = request.Email;
            //Password = request.Password;
            CreatedDate = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            isActive = true;
            Salt = hashTuple.Item1;
            HashedPassword = hashTuple.Item2;
            ImageUrl = null;
            FirstName = request.FirstName;
            LastName = request.LastName;
        }
    }
}
