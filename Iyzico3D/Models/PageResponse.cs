namespace Iyzico3D.Models
{
    public class PageResponse
    {
        public bool Success { get; set; }
        public string? HtmlContent { get; set; }
        public string? Message { get; set; }
        public string? ErrorCode { get; set; }
    }
}
