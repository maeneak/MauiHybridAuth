using Microsoft.AspNetCore.Identity;

namespace MauiHybridAuth.Web.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string FirstName { get; set; } = string.Empty;
        [PersonalData]
        public string LastName { get; set; } = string.Empty;
        public byte[] ProfilePicture { get; set; } = new byte[0];
        public string Initials { 
            get{
                return $"{FirstName[0]}{LastName[0]}";
            }
        } 

        public ICollection<IdentityUserRole<string>> Roles { get; set; }
        public ICollection<IdentityUserClaim<string>> Claims { get; set; }
    }

}
