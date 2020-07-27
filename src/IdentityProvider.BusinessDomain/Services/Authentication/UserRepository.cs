using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityProvider.DataAccess.Entities;
using IdentityProvider.DataAccess.Entities.Enums;
using IdentityServer4.Events;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityProvider.BusinessDomain.Services.Authentication
{
    public interface IUserRepository
    {
        Task<bool> ValidateCredentials(string username, string password);
        Task CreateUser(string firstname, string lastname, string email, string password, RolesConstants.Enum role);
        Task<ApplicationIdentityUser> FindBySubjectId(string subjectId);
        Task<(ApplicationIdentityUser, List<string>)> FindBySubjectIdForClaim(string subjectId);
        Task<ApplicationIdentityUser> FindByUsername(string username);
        Task<(ApplicationIdentityUser, List<string>)> FindByUsernameForClaim(string username);
        Task<bool> ValidatePassword(ApplicationIdentityUser user, string password);
        Task<bool> UserWithEmailExist(string email);
    }
    
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationIdentityUser> _userManager;
        private readonly IEventService _events;
        public UserRepository(UserManager<ApplicationIdentityUser> userManager, IEventService events)
        {
            _userManager = userManager;
            _events = events;
        }

        public async Task<bool> ValidateCredentials(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName));
                return true;
            }

            return false;
        }

        public async Task CreateUser(string firstname, string lastname, string email, string password, RolesConstants.Enum role )
        {
            var roleName = Enum.GetName(typeof(RolesConstants.Enum), role);
            var identityUser = new ApplicationIdentityUser
            {
                UserName = email,
                Id = Guid.NewGuid().ToString(),
                Email = email,
                EmailConfirmed = true,
                FirstName = firstname,
                LastName = lastname,
                CreatedTime = DateTime.UtcNow,
            };

            await _userManager.CreateAsync(identityUser, password);
            await _userManager.AddClaimsAsync(identityUser, new List<Claim>
            {
                new Claim(JwtClaimTypes.Email, email),
                new Claim(JwtClaimTypes.Role, Enum.GetName(typeof(RolesConstants.Enum), role))
            });
            await _userManager.AddToRoleAsync(identityUser, roleName);
        }

        public async Task<ApplicationIdentityUser> FindBySubjectId(string subjectId)
        {
            return await _userManager.FindByIdAsync(subjectId);
        }

        public async Task<(ApplicationIdentityUser, List<string>)> FindBySubjectIdForClaim(string subjectId)
        {
            var user =  await _userManager.FindByIdAsync(subjectId);
            if (user == null)
            {
                return (null, null);
            }

            var roles = await _userManager.GetRolesAsync(user);
            return (user, roles.ToList());
        }

        public async Task<ApplicationIdentityUser> FindByUsername(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<(ApplicationIdentityUser, List<string>)> FindByUsernameForClaim(string username)
        {
            var user =  await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return (null, null);
            }

            var roles = await _userManager.GetRolesAsync(user);
            return (user, roles.ToList());
        }

        public async Task<bool> ValidatePassword(ApplicationIdentityUser user, string password)
        {
            return user != null && await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<bool> UserWithEmailExist(string email)
        {
            return await _userManager.Users.AnyAsync(c => c.Email.ToLower() == email.ToLower()); 
        }
    }
}