using System.ComponentModel.DataAnnotations;

namespace UserCRUD.Models
{
    public class UserChangePasswordRequestModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }

    }
}
