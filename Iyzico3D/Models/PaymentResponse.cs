using System.Text.Json.Serialization;

namespace Iyzico3D.Models
{
    public class PaymentResponse
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }
        [JsonPropertyName("threeDSHtmlContent")]
        public string? ThreeDSHtmlContent { get; set; }
        [JsonPropertyName("errorCode")]
        public string? status { get; set; }
        [JsonPropertyName("errorMessage")]
        public string? ErrorMessage { get; set; }
    }
}
