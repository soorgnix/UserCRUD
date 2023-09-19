using UserCRUD.Models;

namespace UserCRUD.Service
{
    public interface IUserService
    {
        public Task<UserCreateResponseModel> UserCreate(UserCreateRequestModel request);
        public Task<UserChangePasswordResponseModel> UserChangePassword(UserChangePasswordRequestModel request);
        public Task<UserDeleteResponseModel> UserDelete(UserDeleteRequestModel request);
        public Task<UserUpdateResponseModel> UserUpdate(UserUpdateRequestModel request);
        public Task<UserFilterResponseModel> UserFilter(UserFilterRequestModel request);
        public Task<UserLoginResponseModel> UserLogin(UserLoginRequestModel request);
    }
}
