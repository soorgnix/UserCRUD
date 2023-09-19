using System.ComponentModel.DataAnnotations;

namespace UserCRUD.Models
{
    public class UserFilterResponseModel
    {
        public List<UserFilterUserList> UserList { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
    }

    public class UserFilterUserList()
    {
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
