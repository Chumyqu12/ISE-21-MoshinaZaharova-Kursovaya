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

        public async Task AddElement(OrderBindingModel model)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var element = new Order
                    {
                        ClientId = model.ClientId,
                        DateCreate = DateTime.Now,
                        CreditEnd = model.CreditEnd,
                        OrderStatus = OrderStatus.Выполнен
                    };
                    context.Orders.Add(element);
                    await context.SaveChangesAsync();

                    var groupServices = model.ServiceOrders.GroupBy(rec => rec.ServiceId).Select(rec => new ServiceOrderBindingModel
                    {
                        ServiceId = rec.Key,
                        Count = rec.Sum(r => r.Count)
                    });
                    foreach (var groupService in groupServices)
                    {
                        context.ServiceOrders.Add(new ServiceOrder
                        {
                            OrderId = element.Id,
                            Count = groupService.Count,
                            Price = context.Services.Where(rec => rec.Id == groupService.ServiceId).FirstOrDefault().Price,
                            ServiceId = groupService.ServiceId
                        });

                    }
                    await context.SaveChangesAsync();
                    await Task.Run(() => transaction.Commit());
                }
                catch (Exception)
                {
                    await Task.Run(() => transaction.Rollback());
                    throw;
                }
            }
        }

        public async Task DelElement(int id)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    Order element = await context.Orders.FirstOrDefaultAsync(rec => rec.Id == id);
                    if (element != null)
                    {
                        context.ServiceOrders.RemoveRange(
                                            context.ServiceOrders.Where(rec => rec.OrderId == id));
                        context.Orders.Remove(element);
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        throw new Exception("Элемент не найден");
                    }
                    await Task.Run(() => transaction.Commit());
                }
                catch (Exception)
                {
                    await Task.Run(() => transaction.Rollback());
                    throw;
                }
            }
        }

        public async Task<OrderViewModel> GetElement(int id)
        {
            Order element = await context.Orders.FirstOrDefaultAsync(rec => rec.Id == id);
            if (element != null)
            {
                var serviceOrders = await context.ServiceOrders.Where(rec => rec.OrderId == element.Id).Include(rec => rec.Service).Select(rec => new ServiceOrderViewModel
                {
                    Id = rec.Id,
                    ServiceName = rec.Service.ServiceName,
                    Count = rec.Count,
                    Price = rec.Price,
                    Total = rec.Count * rec.Price
                }).ToListAsync();
                var sum = serviceOrders.Select(rec => rec.Total).DefaultIfEmpty(0).Sum();
                var paid = context.Pays.Where(rec => rec.OrderId == element.Id).Select(rec => rec.Summ).DefaultIfEmpty(0).Sum();
                return new OrderViewModel
                {
                    Id = element.Id,
                    ClientFIO = context.Clients.Where(rec => rec.Id == element.ClientId).Select(rec => rec.ClientFIO).FirstOrDefault(),
                    ClientId = element.ClientId,
                    DateCreate = element.DateCreate.ToLongDateString(),
                    CreditEnd = element.DateCreate.ToLongDateString(),
                    ServiceOrders = serviceOrders,
                    OrderStatus = element.OrderStatus.ToString(),
                    Sum = sum,
                    Paid = paid,
                    Credit = sum - paid,
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

        public Task UpdElement(OrderBindingModel model)
        {
            throw new NotImplementedException();
        }
    }
}
