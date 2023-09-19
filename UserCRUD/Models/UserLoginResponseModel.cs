using System.ComponentModel.DataAnnotations;

namespace UserCRUD.Models
{
    public class UserLoginResponseModel
    {
        public string Token { get; set; }
        public DateTime ExpireDate { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
    }
}
