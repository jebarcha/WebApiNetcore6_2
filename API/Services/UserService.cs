using API.Dtos;
using API.Helpers;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
namespace API.Services;

public class UserService : IUserService
{
    private readonly JWT _jwt;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(IUnitOfWork unitOfWork, IOptions<JWT> jwt,
        IPasswordHasher<User> passwordHasher)
    {
        _jwt = jwt.Value;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }
    public async Task<string> RegisterAsync(RegisterDto registerDto)
    {
        var user = new User
        {
            Name = registerDto.Name,
            LastName = registerDto.LastName,
            Email = registerDto.Email,
            Username = registerDto.Username
        };

        user.Password = _passwordHasher.HashPassword(user, registerDto.Password);

        var usuarioExiste = _unitOfWork.Users
                                    .Find(u => u.Username.ToLower() == registerDto.Username.ToLower())
                                    .FirstOrDefault();

        if (usuarioExiste == null)
        {
            var rolPredeterminado = _unitOfWork.Roles
                                    .Find(u => u.Name == Authorization.role_default.ToString())
                                    .First();
            try
            {
                user.Roles.Add(rolPredeterminado);
                _unitOfWork.Users.Add(user);
                await _unitOfWork.SaveAsync();

                return $"User {registerDto.Username} has been registered successfully.";
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return $"Error: {message}";
            }
        }
        else
        {
            return $"User {registerDto.Username} already exists.";
        }
    }

    public async Task<UserDataDto> GetTokenAsync(LoginDto model)
    {
        UserDataDto userDataDto = new UserDataDto();
        var user = await _unitOfWork.Users
                    .GetByUserNameAsync(model.Username);

        if (user == null)
        {
            userDataDto.IsAuthenticated = false;
            userDataDto.Message = $"User does not exists {model.Username}.";
            return userDataDto;
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);

        if (result == PasswordVerificationResult.Success)
        {
            userDataDto.IsAuthenticated = true;
            JwtSecurityToken jwtSecurityToken = CreateJwtToken(user);
            userDataDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            userDataDto.Email = user.Email;
            userDataDto.UserName = user.Username;
            userDataDto.Roles = user.Roles
                                            .Select(u => u.Name)
                                            .ToList();

            if (user.RefreshTokens.Any(a => a.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.Where(a => a.IsActive == true).FirstOrDefault();
                userDataDto.RefreshToken = activeRefreshToken.Token;
                userDataDto.RefreshTokenExpiration = activeRefreshToken.Expires;
            }
            else
            {
                var refreshToken = CreateRefreshToken();
                userDataDto.RefreshToken = refreshToken.Token;
                userDataDto.RefreshTokenExpiration = refreshToken.Expires;
                user.RefreshTokens.Add(refreshToken);
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveAsync();
            }

            return userDataDto;
        }
        userDataDto.IsAuthenticated = false;
        userDataDto.Message = $"Invalid Credentials {user.Username}.";
        return userDataDto;
    }

    public async Task<string> AddRoleAsync(AddRoleDto model)
    {

        var user = await _unitOfWork.Users
                    .GetByUserNameAsync(model.Username);

        if (user == null)
        {
            return $"User does not exists {model.Username}.";
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);
        if (result == PasswordVerificationResult.Success)
        {
            var rolExists = _unitOfWork.Roles
                                        .Find(u => u.Name.ToLower() == model.Role.ToLower())
                                        .FirstOrDefault();
            if (rolExists != null)
            {
                var userHasRole = user.Roles
                                            .Any(u => u.Id == rolExists.Id);
                if (userHasRole == false)
                {
                    user.Roles.Add(rolExists);
                    _unitOfWork.Users.Update(user);
                    await _unitOfWork.SaveAsync();
                }

                return $"Role {model.Role} added to user {model.Username} successfully.";
            }

            return $"Role {model.Role} not found.";
        }
        return $"Invalid credentials {user.Username}.";
    }

    public async Task<UserDataDto> RefreshTokenAsync(string refreshToken)
    {
        var datosUsuarioDto = new UserDataDto();

        var user = await _unitOfWork.Users
                        .GetByRefreshTokenAsync(refreshToken);

        if (user == null)
        {
            datosUsuarioDto.IsAuthenticated = false;
            datosUsuarioDto.Message = $"Token is not associated to any user.";
            return datosUsuarioDto;
        }

        var refreshTokenBd = user.RefreshTokens.Single(x => x.Token == refreshToken);

        if (!refreshTokenBd.IsActive)
        {
            datosUsuarioDto.IsAuthenticated = false;
            datosUsuarioDto.Message = $"Token is not active.";
            return datosUsuarioDto;
        }
        //Revoke current Refresh Token and
        refreshTokenBd.Revoked = DateTime.UtcNow;
        //generate a new Refresh Token and save it in the database
        var newRefreshToken = CreateRefreshToken();
        user.RefreshTokens.Add(newRefreshToken);
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveAsync();
        //Generate a new Json Web Token 😊
        datosUsuarioDto.IsAuthenticated = true;
        JwtSecurityToken jwtSecurityToken = CreateJwtToken(user);
        datosUsuarioDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        datosUsuarioDto.Email = user.Email;
        datosUsuarioDto.UserName = user.Username;
        datosUsuarioDto.Roles = user.Roles
                                        .Select(u => u.Name)
                                        .ToList();
        datosUsuarioDto.RefreshToken = newRefreshToken.Token;
        datosUsuarioDto.RefreshTokenExpiration = newRefreshToken.Expires;
        return datosUsuarioDto;
    }

    private RefreshToken CreateRefreshToken()
    {
        var randomNumber = new byte[32];   //can use ip of client
        using (var generator = RandomNumberGenerator.Create())
        {
            generator.GetBytes(randomNumber);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                Expires = DateTime.UtcNow.AddDays(10),
                Created = DateTime.UtcNow
            };
        }
    }


    private JwtSecurityToken CreateJwtToken(User usuario)
    {
        var roles = usuario.Roles;
        var roleClaims = new List<Claim>();
        foreach (var role in roles)
        {
            roleClaims.Add(new Claim("roles", role.Name));
        }
        var claims = new[]
        {
                                new Claim(JwtRegisteredClaimNames.Sub, usuario.Username),
                                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                                new Claim("uid", usuario.Id.ToString())
                        }
        .Union(roleClaims);
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
            signingCredentials: signingCredentials);
        return jwtSecurityToken;
    }

}

