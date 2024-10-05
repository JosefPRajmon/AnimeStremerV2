using Microsoft.AspNetCore.Identity;

namespace test.Models.AdminSystem
{
    /// <summary>
    /// Represents an application user, extending the base IdentityUser class with additional properties.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        #region Identification
        /// <summary>
        /// Gets or sets the ID of the user's subscription. Can be null if the user doesn't have a subscription.
        /// </summary>
        public int? SubscriptionId { get; set; }
        /// <summary>
        /// Gets or sets the user's subscription. This is a navigation property for the related Subscription entity.
        /// </summary>
        public virtual Subscription Subscription { get; set; }
        /// <summary>
        /// Gets or sets the country of the user.
        /// </summary>
        #endregion
        #region Creators
        public string? Country { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the user is a subtitle creator.
        /// </summary>
        public bool IsSubtitleCreator { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the user is a content creator.
        /// </summary>
        public bool IsContentCreator { get; set; }
        #endregion
    }
}
