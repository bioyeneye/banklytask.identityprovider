using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityProvider.DataAccess.Entities;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;

namespace IdentityProvider.BusinessDomain.Services.Authentication
{
    public class CustomProfileService : IProfileService
    {
        private readonly IUserRepository _userRepository;

        public CustomProfileService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            try
            {
                if (!string.IsNullOrEmpty(context.Subject.Identity.Name))
                {
                    var userData = await _userRepository.FindByUsernameForClaim(context.Subject.Identity.Name);
                    if (userData.Item1 != null)
                    {
                        var claims = GetUserClaims(userData.Item1, userData.Item2);
                        var identityClaim = claims.Where(x => context.RequestedClaimTypes.Contains(x.Type)).ToList();
                        context.IssuedClaims = identityClaim;
                    }
                }
                else
                {
                    var userId = context.Subject.Claims.FirstOrDefault(x => x.Type == "sub");
                    if (!string.IsNullOrEmpty(userId?.Value))
                    {
                        //get user from db (find user by user id)
                        if (userId != null)
                        {
                            var user = await _userRepository.FindBySubjectIdForClaim(userId.Value);

                            // issue the claims for the user
                            if (user.Item1 != null)
                            {
                                var claims = GetUserClaims(user.Item1, user.Item2);
                                context.IssuedClaims = claims.Where(x => context.RequestedClaimTypes.Contains(x.Type)).ToList();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //log your error
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            try
            {
                //get subject from context (set in ResourceOwnerPasswordValidator.ValidateAsync),
                var userId = context.Subject.Claims.FirstOrDefault(x => x.Type == "user_id");

                if (!string.IsNullOrEmpty(userId?.Value))
                {
                    if (userId != null)
                    {
                        var user = await _userRepository.FindBySubjectId(userId.Value);
                        if (user != null)
                        {
                            // if (user.IsActive)
                            // {
                            //     context.IsActive = user.IsActive;
                            // }
                            context.IsActive = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //handle error logging
            }
        }
        
        public static Claim[] GetUserClaims(ApplicationIdentityUser user, List<string> roles = null)
        {
            var claims = new List<Claim>
            {
                new Claim("user_id", user.Id ?? ""),
                new Claim(JwtClaimTypes.Name, (!string.IsNullOrEmpty(user.UserName) && !string.IsNullOrEmpty(user.UserName)) ? (user.FirstName + " " + user.LastName) : ""),
                new Claim(JwtClaimTypes.GivenName, user.FirstName ?? ""),
                new Claim(JwtClaimTypes.FamilyName, user.LastName ?? ""),
                new Claim(JwtClaimTypes.Email, user.Email ?? ""),
            };

            if (roles != null && roles.Count > 0)
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role));
                    claims.Add(new Claim(JwtClaimTypes.Role, role));
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            return claims.ToArray();
        }
    }
}