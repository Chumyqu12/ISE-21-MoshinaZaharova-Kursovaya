using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetService.VetBindingModels;
using VetService.VetViewModels;

namespace VetService.VetInterfaces
{
    public interface  IVisit
    {
        List<VisitViewModel> GetList();

        VisitViewModel GetElement(int id);

        void AddElement(VisitBindingModel model);

        void UpdElement(VisitBindingModel model);

        void DelElement(int id);
    }
}
