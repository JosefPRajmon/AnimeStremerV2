namespace test.Models.AdminSystem
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public Subscription Subscription { get; set; }
        public List<User> ConnectedAccounts { get; set; }
        public bool IsSubtitleCreator { get; set; }
        public bool IsContentCreator { get; set; }
        public bool IsAdmin { get; set; }
    }
}
