using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nunoguz_Boilerplate.DataModels.RequestModels.Users
{
    public class ChangePasswordRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 6)]
        public string NewPassword { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 6)]
        [Compare("NewPassword", ErrorMessage = "Password doesn't match.")]
        public string ConfirmNewPassword { get; set; }
    }
}
