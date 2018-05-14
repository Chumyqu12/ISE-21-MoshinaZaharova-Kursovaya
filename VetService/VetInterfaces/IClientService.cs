using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetService.App;
using VetService.VetBindingModels;
using VetService.VetViewModels;

namespace VetService.VetInterfaces
{
    public interface IClientService
    {
        Task<List<ClientViewModel>> GetList();

        Task<ClientViewModel> GetElement(int id);

        Task AddElement(ClientCreateModel model);

        Task UpdElement(ClientBindingModel model);

        Task DelElement(int id);

        Task<AppUser> GetUser(int id);

        Task<AppUser> GetUserByName(string name);

        Task RaiseBonuses(RetributionBindingModel model);

        Task DecreaseBonuses(RetributionBindingModel model);

        Task PenetrateClients();
    }
}
