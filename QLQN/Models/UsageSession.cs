public class UsageSession
{
    public int Id { get; set; }
    public int UserAccountId { get; set; }
    public int ComputerId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public decimal Cost { get; set; }

   // public User? User { get; set; }
    public Computer Computer { get; set; }
}
