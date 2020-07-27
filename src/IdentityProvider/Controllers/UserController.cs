using System;
using System.Threading.Tasks;
using IdentityProvider.BusinessDomain.Services;
using IdentityProvider.BusinessDomain.Services.Authentication;
using IdentityProvider.BusinessDomain.ViewModel;
using IdentityProvider.DataAccess.Entities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityProvider.Controllers
{
    [Route("api/users")]
    public class UserController : Controller
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        
        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] RegisterUserViewModel model)
        {
            try
            {
                var response = await _userService.CreateUser(model);
                if (!response.IsSuccessful)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
        
        
        // [HttpPost("/admin")]
        // public IActionResult PostAdmin(RegisterViewModel model)
        // {
        //     return Ok();
        // }
    }
}