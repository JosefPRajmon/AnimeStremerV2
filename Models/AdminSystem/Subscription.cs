namespace test.Models.AdminSystem
{
    public class Subscription
    {
        public int Id { get; set; }
        public int DeviceCount { get; set; }
        public int PeopleCount { get; set; }
        public decimal Price { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
    }
}
