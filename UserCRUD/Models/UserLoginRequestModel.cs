using System.ComponentModel.DataAnnotations;

namespace UserCRUD.Models
{
    public class UserLoginRequestModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
