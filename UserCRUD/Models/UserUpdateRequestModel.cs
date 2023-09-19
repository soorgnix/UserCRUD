using System.ComponentModel.DataAnnotations;

namespace UserCRUD.Models
{
    public class UserUpdateRequestModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
