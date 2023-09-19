using System.ComponentModel.DataAnnotations;

namespace UserCRUD.Models
{
    public class UserCreateRequestModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
