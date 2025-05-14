namespace Service2.DTO
{
    public class CreateTrans
    {
        public Guid sender { get; set; }
        public string receiverPhone { get; set; }
        public decimal amount { get; set; }
    }
}
