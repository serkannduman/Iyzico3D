using Iyzico3D.Data;
using Iyzico3D.Entities;
using Iyzico3D.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Iyzico3D.Controllers
{
    public class HomeController : Controller
    {
        private readonly IyzicoSettings _settings;
        private readonly IOrderRepository _orderRepository;
        public HomeController(IOptions<IyzicoSettings> options, IOrderRepository orderRepository)
        {
            _settings = options.Value;
            _orderRepository = orderRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Payment([FromForm] CardModel cardModel)
        {
            //Response için kullanılacak model
            PageResponse pageResponse = new PageResponse();

            //Sipariş oluşturma
            Order order = new()
            {
                OrderNo = Guid.NewGuid().ToString(),
                Price = 1250m,
                PaidPrice = 1250m,
                CreatedDate = DateTime.Now,
                Status = false,
                OrderLines = new List<OrderLine>()
                {
                    new OrderLine()
                    {
                        Name = "Product 1",
                        Price = 500m,
                        Quantity = 1,
                    },
                    new OrderLine()
                    {
                        Name = "Product 2",
                        Price = 750m,
                        Quantity = 1,
                    },
                }
                
            };
            //Ödeme isteği oluşturma
            PaymentRequest paymentRequest = new()
            {
                Locale = "tr",
                ConversationId = order.OrderNo,
                Price = order.Price,
                PaidPrice = order.PaidPrice,
                Currency = "TRY",
                Installment = 1,
                PaymentChannel = "WEB",
                BasketId = order.OrderNo,
                PaymentGroup = "PRODUCT",
                CallbackUrl = "https://localhost:7296/Home/Callback",
                PaymentCard = new PaymentCard
                {
                    CardHolderName = cardModel.CardHolderName ?? "Serkan DUMAN",
                    CardNumber = cardModel.CardNumber!.Replace(" ",""),
                    ExpireYear = cardModel.ExpiryYear!,
                    ExpireMonth = cardModel.ExpiryMonth!,
                    Cvc = cardModel.Cvc!
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
            //BasketItem'ların eklenmesi
            foreach (var orderLine in order.OrderLines)
            {
                BasketItem basketItem = new()
                {
                    Id = orderLine.OrderLineId.ToString(),
                    Price = orderLine.Price,
                    Name = orderLine.Name ?? "Product",
                    Category1 = "Category 1",
                    Category2 = "Category 2",
                    ItemType = "PHYSICAL"
                };
                paymentRequest.BasketItems.Add(basketItem);
            }
            //Client Oluşturma
            using var client = new HttpClient();

            JsonSerializerOptions options = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            //Requestin json formatına dönüştürülmesi
            var json = JsonSerializer.Serialize(paymentRequest,options);

            var authToken = "";

            //Tokenin Headera eklenmesi
            client.DefaultRequestHeaders.Add("Authorization", authToken);
            //İstek içeriğinin oluşturulması
            var content = new StringContent(json,Encoding.UTF8,"application/json");

            var postUrl = _settings.BaseUrl + "/payment/3dsecure/initialize";

            var response = await client.PostAsync(postUrl, content);
            var responseString = await response.Content.ReadAsStringAsync();

            //Response deserialize edilmesi.
            var paymentResponse = JsonSerializer.Deserialize<PaymentResponse>(responseString);

            //Response kontrolü
            if(paymentResponse == null)
            {
                pageResponse.Success = false;
                pageResponse.Message = "Sipariş oluşturulamadı.";
                return View(pageResponse);
            }




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
