using AutoMapper;
using budget_api.Models.DTOs;
using budget_api.Models.Entities;
using budget_api.Repositories.Interface;
using budget_api.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace budget_api.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        public readonly PasswordHasher<User> _passwordHasher;
        public readonly IUserRepository _userRepository;
        public readonly IConfiguration _configuration;
        public readonly IMapper _mapper;

        public AuthenticationService(IConfiguration configuration ,PasswordHasher<User> passwordHasher, IUserRepository userRepository, IMapper mapper)
        {
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
            _configuration = configuration;
            _mapper = mapper;
        }


        public async Task<ActionResult<String?>?> AuthenticateAsync(AuthenticationRequestBodyDto authRequestBody)
        {
            //Step1: Validate the user credentials
            var validatedUser = await ValidateUserCredentials(authRequestBody.Email, authRequestBody.Password);
            if (validatedUser == null)
            {
                return null;
            }

            //Step:2 Create a security Key & Signing Credentials
            var securityKey =
                new SymmetricSecurityKey(
                    Convert.FromBase64String(
                        _configuration["Authentication:SecretForKey"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //The claims
            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("sub", validatedUser.Id.ToString()));
            claimsForToken.Add(new Claim("given_name", validatedUser.FirstName));
            claimsForToken.Add(new Claim("family_name", validatedUser.LastName));
            claimsForToken.Add(new Claim("email", validatedUser.Email));
            claimsForToken.Add(new Claim("userId", validatedUser.Id.ToString()));

            //Create a Token 
            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials
                );

            var token = new JwtSecurityTokenHandler()
                .WriteToken(jwtSecurityToken);

            return token;
        }

        public async Task<ActionResult<User>> RegisterAsync(UserCreationDto user)
        {
            //Create new User
            User mappedUser = _mapper.Map<User>(user);

            //Hash Password
            var hashedPassword = HashPassword(mappedUser);
            mappedUser.Password = hashedPassword;
            mappedUser.CreatedAt = DateTime.UtcNow;

            //Store new user
            await _userRepository.AddUser(mappedUser);
            //Save Changes to DB
            await _userRepository.SaveChangesAsync();
            return mappedUser;
        }

        private async Task<User?> ValidateUserCredentials(string email, string password)
        {
            //Check if User Exist
            var retrievedUser = await _userRepository.GetUserByEmailAsync(email);
            if (retrievedUser == null)
            {
                return null;
            }

            //Verify Password
            bool isPasswordVerified = VerifyPassword(retrievedUser, password);
            if (!isPasswordVerified)
            {
                return null;
            }

            return retrievedUser;

        }
        private string HashPassword(User user)
        {
            var hashedPassword =  _passwordHasher.HashPassword(user, user.Password);
            return hashedPassword;
        }
        private bool VerifyPassword(User user, string password)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            return result == PasswordVerificationResult.Success;
        }
    }
}
