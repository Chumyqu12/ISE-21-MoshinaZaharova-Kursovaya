using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using VetService.VetBindingModels;
using VetService.VetInterfaces;

namespace VetRestApi.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportController : ApiController
    {
        private readonly IReportService service;

        private readonly string TempPath;

        private readonly string ResourcesPath;

        public ReportController(IReportService service)
        {
            this.service = service;
            TempPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Temp/");
            ResourcesPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Resources/");
        }

        [HttpPost]
        public async Task SendClientAccountDoc(ReportBindingModel model)
        {
            model.FileName = TempPath;
            await service.SendClientAccountDoc(model);
        }

        [HttpPost]
        public async Task SendClientAccountXls(ReportBindingModel model)
        {
            model.FileName = TempPath;
            await service.SendClientAccountXls(model);
        }

        [HttpPost]
        public async Task SendClientsCredits(ReportBindingModel model)
        {
            model.FileName = TempPath;
            await service.SendClientsCredits(model);
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetPays(ReportBindingModel model)
        {
            var list = await service.GetPays(model);
            if (list == null)
            {
                InternalServerError(new Exception("Нет данных"));
            }
            return Ok(list);
        }

        [HttpPost]
        public async Task SavePays(ReportBindingModel model)
        {
            model.FontPath = ResourcesPath + "TIMCYR.TTF";
            if (!File.Exists(model.FontPath))
            {
                File.WriteAllBytes(model.FontPath, Properties.Resources.TIMCYR);
            }
            await service.SavePays(model);
        }

    }
}
