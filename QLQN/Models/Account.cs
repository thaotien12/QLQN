namespace QLQN.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }

        // Navigation
        public User User { get; set; }
    }

}
