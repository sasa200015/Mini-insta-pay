using Microsoft.EntityFrameworkCore;

namespace Service3.Model
{
    public class Report
    {
        public int Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string transactionType { get; set; }
        public decimal amount { get; set; }
        public DateTime transactionDate { get; set; }
    }
}
