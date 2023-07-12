﻿using ePizzaHub.Core.Entities;
using ePizzaHub.Repositories.Interfaces;
using ePizzaHub.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Razorpay.Api;
using System.Security.Cryptography;
using System.Text;

namespace ePizzaHub.Services.Implementations
{
    public class PaymentService : Service<PaymentDetail>, IPaymentService
    {
        RazorpayClient _client;
        IRepository<PaymentDetail> _paymentRepo;
        ICartRepository _cartRepository;
        IConfiguration _configuration;
        public PaymentService(IConfiguration configuration, IRepository<PaymentDetail> paymentRepo, ICartRepository cartRepo) : base(paymentRepo)
        {
            _paymentRepo= paymentRepo;
            _cartRepository= cartRepo;
            _configuration = configuration;
            _client = new RazorpayClient(_configuration["RazorPay:key"], _configuration["RazorPay:Secret"]);
        }

        public string CreateOrder(decimal amount, string currency, string receipt)
        {

            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", amount); // amount in the smallest currency unit
            options.Add("receipt", receipt);
            options.Add("currency", currency);
            Razorpay.Api.Order order = _client.Order.Create(options);

            return order["id"];
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
            _paymentRepo.Add(model);
            Cart cart = _cartRepository.Find(model.CartId);
            cart.IsActive = false;
            return _paymentRepo.SaveChanges();
        }
    }
}
