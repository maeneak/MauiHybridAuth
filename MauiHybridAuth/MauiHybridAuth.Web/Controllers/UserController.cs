using MauiHybridAuth.Shared.Models;
using MauiHybridAuth.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MauiHybridAuth.Web.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            return base.Ok(new Shared.Models.UserInfo() {
                Id = user.Id, 
                Email = user.Email!, 
                Fullname = user.Fullname, 
                PreferredName = user.PreferredName,
                Initials = user.Initials!,
                ProfilePicture = user.ProfilePictureBase64 });
        }

        [HttpPost("setpicture")]
        public async Task<IActionResult> UploadProfilePicture([FromForm] IFormFile file)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                user.ProfilePicture = memoryStream.ToArray();
            }

            await _userManager.UpdateAsync(user);
            return Ok(new { message = "Profile picture updated successfully!" });
        }

    }
}
