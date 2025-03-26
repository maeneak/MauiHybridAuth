using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauiHybridAuth.Web.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string Fullname { get; set; } = string.Empty;
        [PersonalData]
        public string PreferredName { get; set; } = string.Empty;
        
        public byte[] ProfilePicture { get; set; } = Array.Empty<byte>();
        public string Initials { get; set; } = string.Empty;
        [NotMapped]
        public IEnumerable<string> Roles { get; set; } = new List<string>();
        [NotMapped]
        public string ProfilePictureBase64 { 
            get {
                return !ProfilePicture.IsNullOrEmpty() ? $"data:image/*;base64,{Convert.ToBase64String(ProfilePicture)}" : string.Empty;
            } 
        }

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; } = new List<IdentityUserClaim<string>>();
        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; } = new List<IdentityUserLogin<string>>();
        public virtual ICollection<IdentityUserToken<string>> Tokens { get; set; } = new List<IdentityUserToken<string>>();
        public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; } = new List<IdentityUserRole<string>>();
    }

}
