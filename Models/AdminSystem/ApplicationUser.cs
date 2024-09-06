using Microsoft.AspNetCore.Identity;

namespace test.Models.AdminSystem
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsSubtitleCreator { get; set; }
        public bool IsContentCreator { get; set; }
        public int? SubscriptionId { get; set; }
        public virtual Subscription Subscription { get; set; }
    }
}
