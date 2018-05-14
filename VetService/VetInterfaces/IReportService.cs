using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetService.VetBindingModels;
using VetService.VetViewModels;

namespace VetService.VetInterfaces
{
    public interface IReportService
    {
        Task<List<ClientCreaditViewModel>> GetClientCredits(ReportBindingModel model);

        Task SendClientCreditDoc(ClientCreaditViewModel model, string TempPath);

        Task SendClientAccountXls(ReportBindingModel model);

        Task SendClientAccountDoc(ReportBindingModel model);

        Task SendClientsCredits(ReportBindingModel model);

        Task<List<PayViewModel>> GetPays(ReportBindingModel model);

        Task SendMail(string mailto, string caption, string message, string path = null);

        Task SavePays(ReportBindingModel model);
    }
}
