using ePizaaHub.API.Filters;
using ePizzaHub.Models;
using ePizzaHub.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ePizaaHub.API.Controllers
{
    [CustomAuthorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        IItemService _itemService;
        public CatalogController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet]
        public IEnumerable<ItemModel> Get()
        {
            return _itemService.GetItems();
        }
    }
}
