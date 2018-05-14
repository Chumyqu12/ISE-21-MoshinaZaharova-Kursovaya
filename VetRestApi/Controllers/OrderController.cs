using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using VetService.VetBindingModels;
using VetService.VetInterfaces;

namespace VetRestApi.Controllers
{
    [Authorize(Roles ="Admin")]
    public class OrderController : ApiController
    {
        private readonly IOrderService service;

        public OrderController(IOrderService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetList()
        {
            var list = await service.GetList();
            if (list == null)
            {
                InternalServerError(new Exception("Нет данных"));
            }
            return Ok(list);
        }

        [HttpGet]
        [Route("api/Order/GetClientList/{clientId}")]
        public async Task<IHttpActionResult> GetClientList(int clientId)
        {
            var list = await service.GetList(clientId);
            if (list == null)
            {
                InternalServerError(new Exception("Нет данных"));
            }
            return Ok(list);
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get(int id)
        {
            var element = await service.GetElement(id);
            if (element == null)
            {
                InternalServerError(new Exception("Нет данных"));
            }
            return Ok(element);
        }

        [HttpPost]
        public async Task AddElement(OrderBindingModel model)
        {
            await service.AddElement(model);
        }

        [HttpDelete]
        public async Task DelElement(int id)
        {
            await service.DelElement(id);
        }
    }
}
