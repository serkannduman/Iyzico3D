namespace Iyzico3D.Models
{
    public class CardModel
    {
        public string? CardHolderName { get; set; }
        public string? CardNumber { get; set; }
        public string? ExpiryYear { get; set; }
        public string? ExpiryMonth { get; set; }
        public string? Cvc { get; set; }
        public int Installment { get; set; }
    }
}
