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
    public interface IAdminService
    {
        Task<List<AdminViewModel>> GetList();

        Task<AdminViewModel> GetElement(int id);

        Task AddElement(AdminCreateModel model);

        Task UpdElement(AdminBindingModel model);

        Task DelElement(int id);

        Task<AppUser> GetUser(int id);

        Task<AppUser> GetUserByName(string name);

        Task BackUp();

    }
}
