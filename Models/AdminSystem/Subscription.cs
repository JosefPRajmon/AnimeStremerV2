namespace test.Models.AdminSystem
{
    /// <summary>
    /// Represents a subscription plan in the application.
    /// </summary>
    public class Subscription
    {
        /// <summary>
        /// Gets or sets the unique identifier for the subscription.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the number of devices allowed for this subscription.
        /// </summary>
        public int DeviceCount { get; set; }
        /// <summary>
        /// Gets or sets the number of people allowed to use this subscription.
        /// </summary>
        public int PeopleCount { get; set; }
        /// <summary>
        /// Gets or sets the price of the subscription.
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Gets or sets the collection of users associated with this subscription.
        /// This is a navigation property for the related ApplicationUser entities.
        /// </summary>
        public virtual ICollection<ApplicationUser> Users { get; set; }
    }
}
