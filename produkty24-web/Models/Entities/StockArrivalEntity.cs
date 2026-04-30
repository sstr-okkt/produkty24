namespace Produkty24_Web.Models.Entities
{
    public class StockArrivalEntity
    {
        public int? Id { get; set; }
        public DateTime Date { get; set; }
        public int StockItemId { get; set; }
        public StockItemEntity StockItem { get; set; }
        public float Quantity { get; set; }
    }
}
