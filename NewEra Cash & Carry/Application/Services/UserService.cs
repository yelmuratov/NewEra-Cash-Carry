using AutoMapper;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using NewEra_Cash___Carry.Application.Interfaces;
using NewEra_Cash___Carry.Core.DTOs.user;
using NewEra_Cash___Carry.Core.Entities;
using NewEra_Cash___Carry.Shared.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NewEra_Cash___Carry.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IMapper _mapper;
        private readonly AuthSettings _authSettings;

        public UserService(IUserRepository userRepository, IRepository<Role> roleRepository, IMapper mapper, AuthSettings authSettings)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
            _authSettings = authSettings;
        }

        public async Task RegisterUserAsync(UserRegisterDto userRegisterDto)
        {
            if ((await _userRepository.FindAsync(u => u.PhoneNumber == userRegisterDto.PhoneNumber)).Any())
            {
                throw new Exception("A user with this phone number already exists.");
            }

            var user = _mapper.Map<User>(userRegisterDto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userRegisterDto.Password);

            var defaultRole = (await _roleRepository.FindAsync(r => r.Name == "Customer")).FirstOrDefault();
            if (defaultRole != null)
            {
                user.UserRoles = new List<UserRole>
                {
                    new UserRole { RoleId = defaultRole.Id }
                };
            }

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task<string> LoginUserAsync(UserLoginDto userLoginDto)
        {
            var dbUser = await _userRepository.GetByPhoneNumberAsync(userLoginDto.PhoneNumber);
            if (dbUser == null || !BCrypt.Net.BCrypt.Verify(userLoginDto.Password, dbUser.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid phone number or password.");
            }

            return GenerateJwtToken(dbUser);
        }

        public async Task LogoutUserAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken != null)
            {
                var expiration = jwtToken.ValidTo;

                var blacklistedToken = new BlacklistedToken
                {
                    Token = token,
                    Expiration = expiration
                };

                await _userRepository.AddBlacklistedTokenAsync(blacklistedToken); // Save token
                await _userRepository.SaveChangesAsync(); // Commit changes
            }
        }


        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserWithRolesAsync(id);
            if (user == null)
            {
                return null;
            }

            return _mapper.Map<UserDto>(user);
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_authSettings.Secret);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber)
            };

            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
