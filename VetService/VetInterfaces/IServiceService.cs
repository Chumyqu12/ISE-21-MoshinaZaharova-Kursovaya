using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetService.VetBindingModels;
using VetService.VetViewModels;

namespace VetService.VetInterfaces
{
    public interface IServiceService
    {
        Task<List<ServiceViewModel>> GetList();

        Task<ServiceViewModel> GetElement(int id);

        Task AddElement(ServiceBindingModel model);

        Task UpdElement(ServiceBindingModel model);

        Task DelElement(int id);
    }
}
