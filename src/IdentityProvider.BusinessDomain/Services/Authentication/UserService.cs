using System;
using System.Net;
using System.Threading.Tasks;
using IdentityProvider.BusinessDomain.ServiceModels;
using IdentityProvider.BusinessDomain.ViewModel;
using IdentityProvider.DataAccess.Entities.Enums;

namespace IdentityProvider.BusinessDomain.Services.Authentication
{
    
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ServiceResponse<string>> CreateUser(RegisterUserViewModel model)
        {
            try
            {
                var emailExist = await _userRepository.UserWithEmailExist(model.Email);
                if (emailExist)
                {
                    var errorMessage = Constants.USER_EMAIL_EXIST.Replace("{email}", model.Email);
                    return new ServiceResponse<string>(false, errorMessage, HttpStatusCode.BadRequest);
                }

                await _userRepository.CreateUser(model.FirstName, model.LastName, model.Email, model.Password, RolesConstants.Enum.User);
                return new ServiceResponse<string>(true, "User created successfully", HttpStatusCode.InternalServerError);
                
            }
            catch (Exception e)
            {
                return new ServiceResponse<string>(false, e.Message, HttpStatusCode.InternalServerError);
            }
        }
    }

    public interface IUserService
    {
        Task<ServiceResponse<string>> CreateUser(RegisterUserViewModel model);
    }
}