using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauiHybridAuth.Web.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string FirstName { get; set; } = string.Empty;
        [PersonalData]
        public string? LastName { get; set; } = string.Empty;
        public byte[] ProfilePicture { get; set; } = new byte[0];
        public string? Initials { get; set; } = string.Empty;
        [NotMapped]
        public IEnumerable<IdentityRole> Roles { get; set; } = new List<IdentityRole>();

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; } = new List<IdentityUserClaim<string>>();
        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; } = new List<IdentityUserLogin<string>>();
        public virtual ICollection<IdentityUserToken<string>> Tokens { get; set; } = new List<IdentityUserToken<string>>();
        public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; } = new List<IdentityUserRole<string>>();
    }

}
