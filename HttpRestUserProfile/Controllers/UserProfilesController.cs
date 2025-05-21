using AccountProfileServiceProvider.Dto;
using AccountProfileServiceProvider.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace HttpRestUserProfile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfilesController(UserProfileServices accountProfileServices) : ControllerBase
    {
        private readonly UserProfileServices _accountProfileServices = accountProfileServices;

        [HttpPost]
        public IActionResult Create(AddAcountProfileForm formData)
        {

            var result = _accountProfileServices.AddUserProfileAsync(formData, formData.AppUserId);
            if (result != null)
            {
                Ok(result);
            }

            return BadRequest(formData);
        }

        [HttpPut]
        public IActionResult Update(UpdateAcountProfileForm formData)
        {

            var result = _accountProfileServices.UpdateUserProfile(formData);
            if (result != null)
            {
                Ok(result);
            }

            return BadRequest(formData);
        }
    }
}
