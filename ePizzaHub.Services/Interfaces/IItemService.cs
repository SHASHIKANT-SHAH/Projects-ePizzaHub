using ePizzaHub.Core.Entities;
using ePizzaHub.Models;

namespace ePizzaHub.Services.Interfaces
{
    public interface IItemService: IService<Item>
    {
        public IEnumerable<ItemModel> GetItems();
    }
}
