namespace Produkty24_Web.Models.Entities
{
    public class OrderEntity
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int ClientId { get; set; }
        public ClientEntity Client { get; set; }
        public int StatusId { get; set; }
        public OrderStatusEntity Status { get; set; }
        public string? Notes { get; set; }
    }
}
