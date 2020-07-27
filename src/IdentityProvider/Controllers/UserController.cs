using System;
using System.Threading.Tasks;
using IdentityProvider.BusinessDomain.Services;
using IdentityProvider.BusinessDomain.Services.Authentication;
using IdentityProvider.DataAccess.Entities.Enums;
using IdentityProvider.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityProvider.Controllers
{
    [Route("api/users")]
    public class UserController : Controller
    {
        private IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> PostUser(RegisterViewModel model)
        {
            try
            {
                var emailExist = await _userRepository.UserWithEmailExist(model.Email);
                if (emailExist)
                {
                    return BadRequest(Constants.USER_EMAIL_EXIST.Replace("{email}", model.Email));
                }

                await _userRepository.CreateUser(model.FirstName, model.LastName, model.Email, model.Password, RolesConstants.Enum.User);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
        
        
        [HttpPost("/admin")]
        public IActionResult PostAdmin(RegisterViewModel model)
        {
            return Ok();
        }
    }
}