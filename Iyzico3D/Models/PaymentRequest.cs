using System.Net;
using System;

namespace Iyzico3D.Models
{
    public class PaymentRequest
    {
        public string Locale { get; set; }
        public string ConversationId { get; set; }
        public decimal Price { get; set; }
        public decimal PaidPrice { get; set; }
        public string Currency { get; set; }          // TRY
        public int Installment { get; set; }           // 1
        public string PaymentChannel { get; set; }     // WEB
        public string BasketId { get; set; }
        public string PaymentGroup { get; set; }       // PRODUCT
        public string CallbackUrl { get; set; }

        public PaymentCard PaymentCard { get; set; }
        public Buyer Buyer { get; set; }
        public ShippingAddress ShippingAddress { get; set; }
        public BillingAddress BillingAddress { get; set; }

        public List<BasketItem> BasketItems { get; set; }

        public string PaymentSource { get; set; }      // Test
    }
    public class PaymentCard
    {
        public string CardHolderName { get; set; }
        public string CardNumber { get; set; }
        public string ExpireYear { get; set; }   // "28"
        public string ExpireMonth { get; set; }  // "12"
        public string Cvc { get; set; }
        public int RegisterCard { get; set; }    // 0/1
    }

    public class Buyer
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string IdentityNumber { get; set; }
        public string Email { get; set; }
        public string GsmNumber { get; set; }
        public string RegistrationDate { get; set; }  // ISO string olarak bırakmak daha güvenli
        public string LastLoginDate { get; set; }
        public string RegistrationAddress { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string Ip { get; set; }
    }
    public class ShippingAddress
    {
        public string Address { get; set; }   // JSON: address
        public string ZipCode { get; set; }
        public string ContactName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
    public class BillingAddress
    {
        public string Address { get; set; }   // JSON: address
        public string ZipCode { get; set; }
        public string ContactName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
    public class BasketItem
    {
        public string Id { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; }
        public string Category1 { get; set; }
        public string Category2 { get; set; }
        public string ItemType { get; set; } // PHYSICAL / VIRTUAL
    }

}
