using Iyzico3D.Models;
using Microsoft.AspNetCore.Mvc;

namespace Iyzico3D.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Payment()
        {
            //Ödeme isteği oluşturma
            PaymentRequest paymentRequest = new()
            {
                Locale = "tr",
                ConversationId = "123456789",
                Price = 100.00m,
                PaidPrice = 100.00m,
                Currency = "TRY",
                Installment = 1,
                PaymentChannel = "WEB",
                BasketId = "B67832",
                PaymentGroup = "PRODUCT",
                CallbackUrl = "https://localhost:7296/Home/Callback",
                PaymentCard = new PaymentCard
                {
                    CardHolderName = "John Doe",
                    CardNumber = "5528790000008",
                    ExpireYear = "2030",
                    ExpireMonth = "12",
                    Cvc = "123"
                },
                Buyer = new Buyer
                {
                    Id="BY789",
                    Name = "John",
                    Surname = "Doe",
                    IdentityNumber = "15465484782",
                    Email = "johndue@gmail.com",
                    GsmNumber = "+905435304445",
                    RegistrationDate = "2013-04-21 15:12:09",
                    LastLoginDate = "2015-10-05 12:43:35",
                    RegistrationAddress = "Altunizade Mah. İnci Çıkmazı Sokak No: 3 İç Kapı No: 10 Üsküdar İstanbul",
                    City = "İstanbul",
                    Country = "Turkey",
                    ZipCode = "34732",
                    Ip = "85.34.78.112"
                },
                ShippingAddress = new Address
                {
                    AddressLine = "Altunizade Mah. İnci Çıkmazı Sokak No: 3 İç Kapı No: 10 Üsküdar İstanbul",
                    ZipCode = "34742",
                    ContactName = "Jane Doe",
                    City = "Istanbul",
                    Country = "Turkey",
                },
                BillingAddress = new Address
                {
                    AddressLine = "Altunizade Mah. İnci Çıkmazı Sokak No: 3 İç Kapı No: 10 Üsküdar İstanbul",
                    ZipCode = "34742",
                    ContactName = "Jane Doe",
                    City = "Istanbul",
                    Country = "Turkey",
                },
                BasketItems = new List<BasketItem>()
            };



            return View();
        }

        public IActionResult Callback()
        {
            return View();
        }

        public IActionResult Error(string message)
        {
            return View();
        }

        public IActionResult Success(string message)
        {
            return View();
        }
    }
}
