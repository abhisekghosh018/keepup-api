using KeepUp.Application.Common;
using KeepUp.Application.DTOs;
using KeepUp.Application.Interface;
using KeepUp.Domain.Entities;
using KeepUp.Infrastructure.DbManager;
using KeepUp.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KeepUp.Infrastructure.Implementation
{

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext dbContext, UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<Result<List<GetUsersRequest>>> GetAllUsers()
        {
            var users = await (from user in _userManager.Users
                               join profile in _dbContext.UserProfiles
                               on user.Id equals profile.ApplicationUserId

                               select new GetUsersRequest
                               {
                                   Id = user.Id.ToString(),
                                   Email = user.Email!,
                                   DisplayName = profile.UserDisplayName
                               }).ToListAsync();

            if (users.Count == 0)
            {
                return Result<List<GetUsersRequest>>.Error("No users found.");
            }

            return Result<List<GetUsersRequest>>.Success(users);
        }

        public async Task<Result<string>> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return Result<string>.Error("Invalid email or password.");
            }

            var isPasswordbalid = await _userManager.CheckPasswordAsync(user, password);

            if (!isPasswordbalid)
            {
                return Result<string>.Error("Invalid email or password.");
            }


            var tokn = GenerateJWTToken(user);
            return Result<string>.Success(tokn);
        }

        public async Task<Result<string>> RegisterAsync(
    string email, string password, string displayName, DateOnly? dob)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return Result<string>.Error(
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            try
            {
                var profile = new UserProfile(displayName, user.Id, dob);
                _dbContext.UserProfiles.Add(profile);
                await _dbContext.SaveChangesAsync();

                return Result<string>.Success(user.Id.ToString());
            }
            catch (DbUpdateException ex) when
                (ex.InnerException is PostgresException pg && pg.SqlState == "23505")
            {
                return Result<string>.Error("Email already exists.");
            }
            catch
            {
                return Result<string>.Error("Failed to complete registration.");
            }
        }


        private string GenerateJWTToken(ApplicationUser user)
        {
            // Implementation for JWT token generation goes here.

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

            };

            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));

            var creds = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }

}
