using Iyzico3D.Data;
using Iyzico3D.Entities;
using Iyzico3D.Methods;
using Iyzico3D.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Globalization;
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
            try
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
                        CardNumber = cardModel.CardNumber!.Replace(" ", ""),
                        ExpireYear = cardModel.ExpiryYear!,
                        ExpireMonth = cardModel.ExpiryMonth!,
                        Cvc = cardModel.Cvc!
                    },
                    Buyer = new Buyer
                    {
                        Id = "BY789",
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
                    ShippingAddress = new ShippingAddress
                    {
                        Address = "Altunizade Mah. İnci Çıkmazı Sokak No: 3 İç Kapı No: 10 Üsküdar İstanbul",
                        ZipCode = "34742",
                        ContactName = "Jane Doe",
                        City = "Istanbul",
                        Country = "Turkey",
                    },
                    BillingAddress = new BillingAddress
                    {
                        Address = "Altunizade Mah. İnci Çıkmazı Sokak No: 3 İç Kapı No: 10 Üsküdar İstanbul",
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
                var json = JsonSerializer.Serialize(paymentRequest, options);

                var authToken = IyzıcoAuthHelper.GenerateAuthToken(_settings.ApiKey!, _settings.SecretKey!, "/payment/3dsecure/initialize", json);

                //Tokenin Headera eklenmesi
                client.DefaultRequestHeaders.Add("Authorization", authToken);
                //İstek içeriğinin oluşturulması
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var postUrl = _settings.BaseUrl + "/payment/3dsecure/initialize";

                var response = await client.PostAsync(postUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();

                //Response deserialize edilmesi.
                var paymentResponse = JsonSerializer.Deserialize<PaymentResponse>(responseString);

                //Response kontrolü
                if (paymentResponse == null)
                {
                    pageResponse.Success = false;
                    pageResponse.Message = "Sipariş oluşturulamadı.";
                    return View(pageResponse);
                }

                if (paymentResponse.Status == "success")
                {
                    var result = await _orderRepository.CreateAsync(order);
                    if (!result)
                    {
                        pageResponse.Success = false;
                        pageResponse.Message = "Sipariş oluşturulamadı.";
                        return Json(pageResponse);
                    }

                    pageResponse.Success = true;

                    string htmlContent = string.Empty;

                    if (!string.IsNullOrEmpty(paymentResponse.ThreeDSHtmlContent))
                    {
                        byte[] data = Convert.FromBase64String(paymentResponse.ThreeDSHtmlContent);
                        htmlContent = Encoding.UTF8.GetString(data);
                    }

                    pageResponse.HtmlContent = htmlContent;
                }
                else
                {
                    pageResponse.Success = false;
                    pageResponse.Message = paymentResponse.ErrorMessage;
                    pageResponse.ErrorCode = paymentResponse.ErrorCode;

                    order.Message = $"{paymentResponse.ErrorCode} - {paymentResponse.ErrorMessage}";

                    var result = await _orderRepository.CreateAsync(order);
                    if (!result)
                    {
                        pageResponse.Success = false;
                        pageResponse.Message = order.Message;
                    }
                }



                return Json(pageResponse);
            }
            catch (Exception ex)
            {
                var a = ex.Message;
                throw;
            }
           
        }

        public async Task<IActionResult> Callback()
        {
            //Iyzicodan gelen formun yakalanması
            var form = await Request.ReadFormAsync();

            var status = form["status"].ToString();
            var conversationId = form["conversationId"].ToString();
            var paymentId = form["paymentId"].ToString();
            var errorMessage = form["errorMessage"].ToString();

            if(status == "false")
            {
                return RedirectToAction("Error","Home", new {message = string.IsNullOrEmpty(errorMessage) ? "Ödeme işlemi sırasında bir sorun oluştu" :  errorMessage});
            }

            //Sipariş bulma
            var order = await _orderRepository.GetByOrderNo(conversationId);
            if(order == null)
                return RedirectToAction("Error", "Home", new { message = "Sipariş bulunamadı." });

            PaymentCompletionRequest paymentCompletionRequest = new PaymentCompletionRequest()
            {
                Locale = "tr",
                ConversationId = conversationId,
                PaymentId = paymentId,
                PaidPrice = order.PaidPrice.ToString("0.00",CultureInfo.InvariantCulture),
                BasketId = conversationId,
                Currency = "TRY"
            };

            //Client Oluşturma
            using var client = new HttpClient();

            JsonSerializerOptions options = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            //Requestin json formatına dönüştürülmesi
            var json = JsonSerializer.Serialize(paymentCompletionRequest, options);

            var authToken = IyzıcoAuthHelper.GenerateAuthToken(_settings.ApiKey!, _settings.SecretKey!, "/payment/v2/3dsecure/auth", json);

            //Tokenin Headera eklenmesi
            client.DefaultRequestHeaders.Add("Authorization", authToken);
            //İstek içeriğinin oluşturulması
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var postUrl = _settings.BaseUrl + "/payment/v2/3dsecure/auth";

            var response = await client.PostAsync(postUrl, content);
            var responseString = await response.Content.ReadAsStringAsync();

            //Response deserialize edilmesi.
            var paymentCompletionResponse = JsonSerializer.Deserialize<PaymentCompletionResponse>(responseString);

            if (paymentCompletionResponse == null)
            {
                return RedirectToAction("Error", "Home", new { message = "Ödeme işlemi sırasında bir sorun oluştu." });
            }

            bool updateOrder = false;

            //Ödeme başarılı ise
            if(paymentCompletionResponse.Status == "success")
            {
                order.Status = true;
                order.Message = "Ödeme başarılı";

                updateOrder = await _orderRepository.UpdateAsync(order);
                if (updateOrder)
                    return RedirectToAction("Success", "Home",new {message = $"{order.OrderNo} numaralı siparişiniz oluşturulmuştur."});
            }

            order.Status = false;
            order.Message = $"Ödeme sırasında bir sorun oluştu. Hata kodu : {paymentCompletionResponse.ErrorCode}, Hata mesajı : {paymentCompletionResponse.ErrorMessage}";

            updateOrder = await _orderRepository.UpdateAsync(order);

            if (updateOrder)
                return RedirectToAction("Error", "Home", new { message =string.IsNullOrEmpty(order.Message) ? "Sipariş işlmi sırasında bir sorun oluştu." : order.Message });

            return RedirectToAction("Error", "Home", new { message = "Sipariş işlmi sırasında bir sorun oluştu." });
        }

        public IActionResult Error(string message)
        {
            ResultModel model = new() { Message = message };
            return View(model);
        }

        public IActionResult Success(string message)
        {
            ResultModel model = new() { Message = message};
            return View(model);
        }
    }
}
