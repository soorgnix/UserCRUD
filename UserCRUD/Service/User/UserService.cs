using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Numerics;
using System.Security.Claims;
using System.Text;
using UserCRUD.Models;

namespace UserCRUD.Service
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public UserService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<UserCreateResponseModel> UserCreate(UserCreateRequestModel request)
        {
            IdentityUser user = await  _userManager.FindByNameAsync(request.UserName);
            if (user != null)
            {
                return new UserCreateResponseModel()
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Message = "Username Already Created"
                };
            }

            IdentityUser newUser = new IdentityUser()
            {
                UserName = request.UserName,
                Email = request.Email,
                SecurityStamp = Guid.NewGuid().ToString(),                
            };

            IdentityResult result = await _userManager.CreateAsync(newUser, request.Password);
            if (result.Succeeded)
            {
                if (!_roleManager.RoleExistsAsync("Admin").Result)
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                }
                IdentityResult assignResult = await _userManager.AddToRoleAsync(newUser, "Admin");

                if (assignResult.Succeeded)
                {
                    return new UserCreateResponseModel()
                    {
                        Status = (int)HttpStatusCode.OK,
                        Message = "Create User Success"
                    };
                }
                else
                {
                    await _userManager.DeleteAsync(newUser);
                    return new UserCreateResponseModel()
                    {
                        Status = (int)HttpStatusCode.InternalServerError,
                        Message = "Save Failed at DB"
                    };
                }
            }
            else
            {
                return new UserCreateResponseModel()
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Message = "Save Failed at DB"
                };
            }
        }

        public async Task<UserUpdateResponseModel> UserUpdate(UserUpdateRequestModel request)
        {
            IdentityUser user = await _userManager.FindByNameAsync(request.UserName);
            if (user != null)
            {
                user.Email = request.Email;

                IdentityResult result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return new UserUpdateResponseModel()
                    {
                        Status = (int)HttpStatusCode.OK,
                        Message = "Update User Success"
                    };
                }
                else
                {
                    return new UserUpdateResponseModel()
                    {
                        Status = (int)HttpStatusCode.InternalServerError,
                        Message = "Save Failed at DB"
                    };
                }

            }
            else
            {
                return new UserUpdateResponseModel()
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Message = "User Not Found"
                };
            }
        }

        public async Task<UserChangePasswordResponseModel> UserChangePassword(UserChangePasswordRequestModel request)
        {
            IdentityUser user = await _userManager.FindByNameAsync(request.UserName);
            if (user != null)
            {
                IdentityResult result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
                if (result.Succeeded)
                {
                    return new UserChangePasswordResponseModel()
                    {
                        Status = (int)HttpStatusCode.OK,
                        Message = "Change Password Success"
                    };
                }
                else
                {
                    return new UserChangePasswordResponseModel()
                    {
                        Status = (int)HttpStatusCode.InternalServerError,
                        Message = "Change Password Failed at DB"
                    };
                }

            }
            else
            {
                return new UserChangePasswordResponseModel()
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Message = "User Not Found"
                };
            }
        }

        public async Task<UserDeleteResponseModel> UserDelete(UserDeleteRequestModel request)
        {
            IdentityUser user = await _userManager.FindByNameAsync(request.UserName);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return new UserDeleteResponseModel()
                    {
                        Status = (int)HttpStatusCode.OK,
                        Message = "Delete User Success"
                    };
                }
                else
                {
                    return new UserDeleteResponseModel()
                    {
                        Status = (int)HttpStatusCode.InternalServerError,
                        Message = "Delete User Failed at DB"
                    };
                }

            }
            else
            {
                return new UserDeleteResponseModel()
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Message = "User Not Found"
                };
            }
        }

        public async Task<UserFilterResponseModel> UserFilter(UserFilterRequestModel request)
        {
            List<UserFilterUserList> users = _userManager.Users.Where(x => !string.IsNullOrEmpty(request.Email) ? x.Email.ToLower().Contains(request.Email) : true)
                .Select(x => new UserFilterUserList()
                {
                    Username = x.UserName,
                    Email = x.Email
                }).ToList();

            return new UserFilterResponseModel()
            {
                UserList = users,
                Status = (int)HttpStatusCode.OK,
                Message = "Listing User Success"
            };
        }

        public async Task<UserLoginResponseModel> UserLogin(UserLoginRequestModel request)
        {
            IdentityUser user = _userManager.FindByNameAsync(request.UserName).Result;
            if (user != null)
            {
                bool isAuth = _userManager.CheckPasswordAsync(user, request.Password).Result;

                if (isAuth)
                {
                    var authClaims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["JWT:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("Username", user.UserName),
                        new Claim("Email", user.Email)
                    };
                    
                    IList<string> userRoles = await _userManager.GetRolesAsync(user);
                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

                    var token = new JwtSecurityToken(
                        issuer: _configuration["JWT:Issuer"],
                        audience: _configuration["JWT:Audience"],
                        expires: DateTime.Now.AddHours(1),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                    return new UserLoginResponseModel
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                        ExpireDate = token.ValidTo,
                        Message = "Login Success",
                        Status = (int)HttpStatusCode.OK
                    };
                }
            }
            return new UserLoginResponseModel
            {
                Message = "Login Failed, Check Your Username or Password",
                Status = (int)HttpStatusCode.BadRequest
            };

        }
    }
}
