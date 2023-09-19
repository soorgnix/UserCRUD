using System.ComponentModel.DataAnnotations;

namespace UserCRUD.Models
{
    public class UserDeleteRequestModel
    {
        [Required]
        public string UserName { get; set; }
    }
}
