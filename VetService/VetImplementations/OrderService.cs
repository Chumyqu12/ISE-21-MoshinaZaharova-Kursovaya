using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetModel;
using VetService.VetBindingModels;
using VetService.VetInterfaces;
using VetService.VetViewModels;

namespace VetService.VetImplementations
{
    public class OrderService : IOrderService
    {
        private readonly AbstractDbContext context;

        public OrderService(AbstractDbContext context)
        {
            this.context = context;
        }

        public void AddElement(OrderBindingModel model)
        {
            context.Orders.Add(new Order
            {
                ClientId = model.ClientId,
                VisitId = model.VisitId,
                DateCreate = DateTime.Now,
                Count = model.Count,
                Summa = model.Summa,
                
            });
            context.SaveChanges();

        }


        public async Task<OrderViewModel> GetElement(int id)
        {
            Order element = await context.Orders.FirstOrDefaultAsync(rec => rec.Id == id);
            if (element != null)
            {
             
               
                var serviceOrders = await context.Orders.Where(rec => rec.Id == element.Id).Include(rec => rec.Visit).Select(rec => new OrderViewModel
                {
                    Id = element.Id,
                    ClientFIO = context.Clients.Where(recC => recC.Id == element.ClientId).Select(recC => recC.ClientFIO).FirstOrDefault(),
                    ClientId = element.ClientId,
                    DateCreate = element.DateCreate.ToLongDateString(),
                    CreditEnd = element.DateCreate.ToLongDateString(),
                    Id = rec.Id,
                    VisitName = rec.Visit.VisitName,
                    Count = rec.Count,
                    Sum = rec.Summa,
                    Total = rec.Count * rec.Summa,
                    OrderStatus = element.OrderStatus.ToString(),
                    CreditDate = element.CreditEnd
                };
            }
            throw new Exception("Элемент не найден");
        }

        public async Task<List<OrderViewModel>> GetList()
        {
            return await context.Orders.Include(rec => rec.Client)//.Include(rec=>rec.ServiceOrders).Include(rec=>rec.Pays)
                .Select(rec => new OrderViewModel
                {
                    Id = rec.Id,
                    ClientFIO = rec.Client.ClientFIO,
                    Mail = rec.Client.UserName,
                    ClientId = rec.ClientId,
                    DateCreate = SqlFunctions.DateName("dd", rec.DateCreate) + " " +
                                            SqlFunctions.DateName("mm", rec.DateCreate) + " " +
                                            SqlFunctions.DateName("yyyy", rec.DateCreate),
                    CreditEnd = SqlFunctions.DateName("dd", rec.CreditEnd) + " " +
                                            SqlFunctions.DateName("mm", rec.CreditEnd) + " " +
                                            SqlFunctions.DateName("yyyy", rec.CreditEnd),
                    OrderStatus = rec.OrderStatus.ToString(),
                    CreditDate = rec.CreditEnd,
                    Count=rec.Count,
                    Sum=rec.Summa,
                    VisitName=rec.Visit.VisitName,
                    VisitId=rec.VisitId,
                    //Sum = rec.ServiceOrders.Select(r=>r.Price * r.Count).DefaultIfEmpty(0).Sum(),
                    //Paid = rec.Pays.Select(r=>r.Summ).DefaultIfEmpty(0).Sum(),
                    //Credit = rec.ServiceOrders.Select(r => r.Price * r.Count).DefaultIfEmpty(0).Sum() - rec.Pays.Select(r => r.Summ).DefaultIfEmpty(0).Sum()
                }).ToListAsync();
        }

        public async Task<List<OrderViewModel>> GetList(int clientId)
        {
            return await context.Orders.Where(rec => rec.ClientId == clientId).Include(rec => rec.Client).Include(rec=>rec.ServiceOrders)
                .Select(rec => new OrderViewModel
                {
                    Id = rec.Id,
                    ClientFIO = rec.Client.ClientFIO,
                    Mail = rec.Client.UserName,
                    ClientId = rec.ClientId,
                    DateCreate = SqlFunctions.DateName("dd", rec.DateCreate) + " " +
                                            SqlFunctions.DateName("mm", rec.DateCreate) + " " +
                                            SqlFunctions.DateName("yyyy", rec.DateCreate),
                    CreditEnd = SqlFunctions.DateName("dd", rec.CreditEnd) + " " +
                                            SqlFunctions.DateName("mm", rec.CreditEnd) + " " +
                                            SqlFunctions.DateName("yyyy", rec.CreditEnd),
                    OrderStatus = rec.OrderStatus.ToString(),
                    CreditDate = rec.CreditEnd,
                    Sum = rec.ServiceOrders.Select(r=>r.Price * r.Count).DefaultIfEmpty(0).Sum(),
                    Paid = rec.Pays.Select(r=>r.Summ).DefaultIfEmpty(0).Sum(),
                    Credit = rec.ServiceOrders.Select(r => r.Price * r.Count).DefaultIfEmpty(0).Sum() - rec.Pays.Select(r => r.Summ).DefaultIfEmpty(0).Sum()
                }).ToListAsync();
        }

        public void FinishOrder(int id)
        {
            Order element = context.Orders.FirstOrDefault(rec => rec.Id == id);
            if (element == null)
            {
                throw new Exception("Элемент не найден");
            }
            element.OrderStatus = OrderStatus.Выполнен;
        }

        public void PayOrder(int id)
        {
            Order element = context.Orders.FirstOrDefault(rec => rec.Id == id);
            if (element == null)
            {
                throw new Exception("Элемент не найден");
            }
            element.OrderStatus = OrderStatus.Оплачен;
        }

        public void ChatPayOrder(int id)
        {
            Order element = context.Orders.FirstOrDefault(rec => rec.Id == id);
            if (element == null)
            {
                throw new Exception("Элемент не найден");
            }
            element.OrderStatus = OrderStatus.Частично_оплачен;
        }

        public Task UpdElement(OrderBindingModel model)
        {
            throw new NotImplementedException();
        }
    }
}
