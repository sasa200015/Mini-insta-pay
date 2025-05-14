namespace Service2.Model
{
    public class Transactions
    {
        public int Id { get; set; }
        public Guid senedrId { get; set; }
        public Guid receiverId{ get; set; }
        public decimal amount { get; set; }
        public string status { get; set; }
        public DateTime createdAt { get; set; }
    }
}
