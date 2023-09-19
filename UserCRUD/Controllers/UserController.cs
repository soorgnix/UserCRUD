using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using UserCRUD.Models;
using UserCRUD.Service;
using UserCRUD.Utils;

namespace UserCRUD.Controllers
{
    [Route("api/user/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenBlackListService _tokenBlackListService;

        public UserController(IUserService userService, ITokenBlackListService tokenBlackListService)
        {
            _userService = userService;
            _tokenBlackListService = tokenBlackListService;
        }

        [HttpPost]
        [Route("create")]
        public async Task<UserCreateResponseModel> UserCreate([FromBody] UserCreateRequestModel request)
        {
            UserCreateResponseModel response = await _userService.UserCreate(request);
            return response;
        }

        [Authorize]
        [HttpPost]
        [Route("update")]
        public async Task<UserUpdateResponseModel> UserUpdate([FromBody] UserUpdateRequestModel request)
        {
            if (_Validate())
            {
                UserUpdateResponseModel response = await _userService.UserUpdate(request);
                return response;
            }
            else
            {
                return new UserUpdateResponseModel()
                {
                    Message = "Please Relogin",
                    Status = (int)HttpStatusCode.Unauthorized
                };
            }
        }

        [Authorize]
        [HttpPost]
        [Route("changepassword")]
        public async Task<UserChangePasswordResponseModel> UserChangePassword([FromBody] UserChangePasswordRequestModel request)
        {
            if (_Validate())
            {
                UserChangePasswordResponseModel response = await _userService.UserChangePassword(request);
                return response;
            }
            else
            {
                return new UserChangePasswordResponseModel()
                {
                    Message = "Please Relogin",
                    Status = (int)HttpStatusCode.Unauthorized
                };
            }
        }

        [Authorize]
        [HttpPost]
        [Route("delete")]
        public async Task<UserDeleteResponseModel> UserUpdate([FromBody] UserDeleteRequestModel request)
        {
            if (_Validate())
            {
                UserDeleteResponseModel response = await _userService.UserDelete(request);
                return response;
            }
            else
            {
                return new UserDeleteResponseModel()
                {
                    Message = "Please Relogin",
                    Status = (int)HttpStatusCode.Unauthorized
                };
            }
        }

        [Authorize]
        [HttpPost]
        [Route("filter")]
        public async Task<UserFilterResponseModel> UserFilter([FromBody] UserFilterRequestModel request)
        {
            if (_Validate())
            {
                UserFilterResponseModel response = await _userService.UserFilter(request);
                return response;
            }
            else
            {
                return new UserFilterResponseModel()
                {
                    Message = "Please Relogin",
                    Status = (int)HttpStatusCode.Unauthorized
                };
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<UserLoginResponseModel> UserLogin([FromBody] UserLoginRequestModel request)
        {
            UserLoginResponseModel response = await _userService.UserLogin(request);
            return response;
        }

        [Authorize]
        [HttpGet]
        [Route("logout")]
        public async Task<UserLogoutResponseModel> UserLogout()
        {
            if (_Validate())
            {
                HttpContext context = HttpContext.Request.HttpContext;
                string token = CommonUtils.GetToken(context);
                await _tokenBlackListService.BlackListToken(token);
                return new UserLogoutResponseModel()
                {
                    Message = "Logout Success",
                    Status = (int)HttpStatusCode.OK
                };
            }
            else
            {
                return new UserLogoutResponseModel()
                {
                    Message = "Logout Failed",
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        private bool _Validate()
        {
            HttpContext context = HttpContext.Request.HttpContext;
            string token = CommonUtils.GetToken(context);
            if (!string.IsNullOrEmpty(token))
            {
                return !_tokenBlackListService.IsBlackListed(token).Result;
            }
            return true;
        }

    }
}
