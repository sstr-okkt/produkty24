namespace Produkty24_Web.Models.Entities
{
    public class StockItemEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? ManufacturerId { get; set; }
        public ManufacturerEntity? Manufacturer { get; set; }
        public string? Description { get; set; }
        public int? CurrencyId { get; set; }
        public CurrencyEntity? Currency { get; set; }
        public float PurchasePrice { get; set; }
        public float RetailPrice { get; set; }
    }
}
