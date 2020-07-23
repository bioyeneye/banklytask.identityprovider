using System;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;

namespace banklytask.identityprovider.Services.Authentication
{
    public class CustomResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        //repository to get user from db
        private readonly IUserRepository _userRepository;

        public CustomResourceOwnerPasswordValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository; 
        }
        
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            try
            {
                //get your user model from db (by username - in my case its email)
                var user = await _userRepository.FindByUsername(context.UserName);
                if (user != null)
                {
                    //check if password match - remember to hash password if stored as hash in db
                    if (await _userRepository.ValidatePassword(user, context.Password)) {
                        //set the result
                        context.Result = new GrantValidationResult(
                            subject: user.Id,
                            authenticationMethod: "pwd", 
                            claims: CustomProfileService.GetUserClaims(user));
                        return;
                    } 

                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Incorrect username or password");
                    return;
                }
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "User does not exist.");
            }
            catch (Exception ex)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid username or password");
            }
        }
    }
}