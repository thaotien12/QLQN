namespace QLQN.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }

        // Foreign key
        public int AccountId { get; set; }
        public Account Account { get; set; }
    }

}
