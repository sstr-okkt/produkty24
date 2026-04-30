namespace Produkty24_Web.ViewModels.Clients
{
    public class ClientProfileViewModel : ClientViewModel
    {
        public string? Address { get; set; }
        public string? Notes { get; set; }
        public int OrdersQuantity { get; set; }
        public float PaymentsTotal { get; set; }
    }
}
