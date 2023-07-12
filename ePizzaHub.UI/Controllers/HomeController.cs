using ePizzaHub.Services.Interfaces;
using ePizzaHub.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace ePizzaHub.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        IItemService _itemService;
        IMemoryCache _memoryCache;
        public HomeController(ILogger<HomeController> logger, IItemService itemService, IMemoryCache memoryCache)
        {
            _logger = logger;
            _itemService = itemService;
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            //var data = _itemService.GetAll();
            //cache
            string key = "catalog";
            var data = _memoryCache.GetOrCreate(key, entry =>
            {
                entry.AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(12);
                return _itemService.GetAll();
            });

            try
            {
                int x = 3, y = 0;
                int z = x / y;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return View(data);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}