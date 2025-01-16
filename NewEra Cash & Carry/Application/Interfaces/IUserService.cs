using NewEra_Cash___Carry.Core.DTOs.user;

namespace NewEra_Cash___Carry.Application.Interfaces
{
    public interface IUserService
    {
        Task RegisterUserAsync(UserRegisterDto userRegisterDto);
        Task<string> LoginUserAsync(UserLoginDto userLoginDto);
        Task LogoutUserAsync(string token);
        Task<UserDto> GetUserByIdAsync(int id);
    }
}
