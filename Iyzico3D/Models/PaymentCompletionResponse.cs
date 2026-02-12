using System.Text.Json.Serialization;

namespace Iyzico3D.Models
{
    public class PaymentCompletionResponse
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }
        [JsonPropertyName("errorCode")]
        public string? ErrorCode { get; set; }
        [JsonPropertyName("errorMessage")]
        public string? ErrorMessage { get; set; }
    }
}
