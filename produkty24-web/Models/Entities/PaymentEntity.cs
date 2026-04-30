namespace Produkty24_Web.Models.Entities
{
    public class PaymentEntity
    {
        public int? Id { get; set; }
        public DateTime Date { get; set; }
        public int ClientId { get; set; }
        public ClientEntity Client { get; set; }
        public int? OrderId { get; set; }
        public OrderEntity Order { get; set; }
        public float Amount { get; set; }
        public string? Notes { get; set; }
    }
}
