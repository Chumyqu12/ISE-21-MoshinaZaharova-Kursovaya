using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetService.VetBindingModels;
using VetService.VetViewModels;

namespace VetService.VetInterfaces
{
    public interface IOrderService
    {
        Task<List<OrderViewModel>> GetList();

        Task<List<OrderViewModel>> GetList(int clientId);

        Task<OrderViewModel> GetElement(int id);

        Task AddElement(OrderBindingModel model);

        Task UpdElement(OrderBindingModel model);

        Task DelElement(int id);

        void ChatPayOrder(int id);

        void PayOrder(int id);

        void FinishOrder(int id);
    }
}
