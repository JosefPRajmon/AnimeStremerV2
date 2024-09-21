namespace test.Models.AdminSystem
{
    /// <summary>
    /// Represents a user in the application.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the hashed password of the user.
        /// This should never store the plain text password.
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Gets or sets the subscription associated with the user.
        /// </summary>
        public Subscription Subscription { get; set; }

        /// <summary>
        /// Gets or sets the list of connected user accounts.
        /// This could represent family members or friends sharing the account.
        /// </summary>
        public List<User> ConnectedAccounts { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has subtitle creation privileges.
        /// </summary>
        public bool IsSubtitleCreator { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has content creation privileges.
        /// </summary>
        public bool IsContentCreator { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has administrative privileges.
        /// </summary>
        public bool IsAdmin { get; set; }
    }
}
