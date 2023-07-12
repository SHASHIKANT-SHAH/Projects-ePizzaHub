using ePizzaHub.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Net.Http.Headers;
namespace ePizzaHub.UI.Controllers
{
    public class ItemController : BaseController
    {
        HttpClient _client;
        IConfiguration _configuration;
        public ItemController(IConfiguration configuration)
        {
            _configuration= configuration;
            _client= new HttpClient();
            _client.BaseAddress = new Uri(_configuration["ApiAddress"]);
        }
        public IActionResult Index()
        {
            IEnumerable<ItemModel>items= new List<ItemModel>();
            if (CurrentUser != null)
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CurrentUser.Token);

                var response = _client.GetAsync(_client.BaseAddress + "/catalog/get").Result;
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    items = JsonSerializer.Deserialize<IEnumerable<ItemModel>>(data);
                }
            }
            return View(items);
        }
    }
}
