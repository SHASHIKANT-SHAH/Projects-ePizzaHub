using ePizzaHub.Core.Entities;
using ePizzaHub.Repositories.Interfaces;
using ePizzaHub.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ePizzaHub.Services.Implementations
{
    public class PaymentService : Service<PaymentDetail>, IPaymentService
    {
        IRepository<PaymentDetail> _paymentRepository;
        ICartRepository _cartRepo;
        private readonly RazorpayClient _client;
        IConfiguration _configuration;
        public PaymentService(IRepository<PaymentDetail> paymentRepo, ICartRepository cartRepo, IConfiguration configuration) : base(paymentRepo)
        {
            _paymentRepository = paymentRepo;
            _cartRepo = cartRepo;
            _configuration = configuration;
            _client = new RazorpayClient(_configuration["RazorPay:Key"], _configuration["RazorPay:Secret"]);
        }

        public string CreateOrder(decimal amount, string currency, string receipt)
        {
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", amount); // amount in the smallest currency unit
            options.Add("receipt", receipt);
            options.Add("currency", currency);
            Razorpay.Api.Order order = _client.Order.Create(options);
            return order["id"].ToString();
        }

        public Payment GetPaymentDetails(string paymentId)
        {
            return _client.Payment.Fetch(paymentId);
        }

        public bool VerifySignature(string signature, string orderId, string paymentId)
        {
            string payload = string.Format("{0}|{1}", orderId, paymentId);
            string secret = RazorpayClient.Secret;
            string actualSignature = getActualSignature(payload, secret);
            return actualSignature.Equals(signature);
        }
        private static string getActualSignature(string payload, string secret)
        {
            byte[] secretBytes = StringEncode(secret);
            HMACSHA256 hashHmac = new HMACSHA256(secretBytes);
            var bytes = StringEncode(payload);

            return HashEncode(hashHmac.ComputeHash(bytes));
        }
        private static byte[] StringEncode(string text)
        {
            var encoding = new ASCIIEncoding();
            return encoding.GetBytes(text);
        }
        private static string HashEncode(byte[] hash)
        {
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
        public int SavePaymentDetails(PaymentDetail model)
        {
            _paymentRepository.Add(model);
            Cart cart = _cartRepo.Find(model.CartId);
            cart.IsActive = false;
            return _paymentRepository.SaveChanges();
        }
    }
}
