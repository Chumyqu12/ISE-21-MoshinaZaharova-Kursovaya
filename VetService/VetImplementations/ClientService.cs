using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VetModel;
using VetService.App;
using VetService.VetBindingModels;
using VetService.VetInterfaces;
using VetService.VetViewModels;

namespace VetService.VetImplementations
{
    public class ClientService : IClientService
    {
        private AbstractDbContext context;

        public ClientService(AbstractDbContext context)
        {
            this.context = context;
        }

        public static ClientService Create(AbstractDbContext context)
        {
            return new ClientService(context);
        }

        public async Task AddElement(ClientCreateModel model)
        {
            Client element = await context.Clients.FirstOrDefaultAsync(rec => rec.ClientFIO == model.ClientFIO);
            if (element != null)
            {
                throw new Exception("Already have a client with such a name");
            }
            element = await context.Clients.FirstOrDefaultAsync(rec => rec.UserName == model.UserName);
            if (element != null)
            {
                throw new Exception("Already have a client with such a email");
            }
            context.Clients.Add(new Client
            {
                ClientFIO = model.ClientFIO,
                UserName = model.UserName,
                Bonuses = 0,
                PasswordHash = model.PasswordHash,
                SecurityStamp = model.SecurityStamp,
                Active = true
            });
            await context.SaveChangesAsync();
        }

        public async Task DecreaseBonuses(RetributionBindingModel model)
        {
            Client element = await context.Clients.FirstOrDefaultAsync(rec => rec.Id == model.ClientId);
            if (element == null)
            {
                throw new Exception("Элемент не найден");
            }
            element.Bonuses -= model.Penalty;
            await context.SaveChangesAsync();
        }

        public async Task DelElement(int id)
        {
            Client element = await context.Clients.FirstOrDefaultAsync(rec => rec.Id == id);
            if (element != null)
            {
                context.Clients.Remove(element);
                await context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Элемент не найден");
            }
        }

        public async Task<ClientViewModel> GetElement(int id)
        {
            Client element = await context.Clients.FirstOrDefaultAsync(rec => rec.Id == id);
            if (element != null)
            {
                return new ClientViewModel
                {
                    Id = element.Id,
                    ClientFIO = element.ClientFIO,
                    UserName = element.UserName,
                    Bonuses = element.Bonuses,
                    Active = (element.Active) ? "Active" : "Locked"
                };
            }
            throw new Exception("Element not found");
        }

        public async Task<List<ClientViewModel>> GetList()
        {
            List<ClientViewModel> result = await context.Clients.Select(rec => new ClientViewModel
            {
                Id = rec.Id,
                ClientFIO = rec.ClientFIO,
                UserName = rec.UserName,
                Bonuses = rec.Bonuses,
                Active = (rec.Active) ? "Active" : "Locked"
            })
                .ToListAsync();
            return result;
        }

        public async Task PenetrateClients()
        {
            DateTime now = DateTime.Now;
            var clients = await context.Orders.Where(rec => rec.CreditEnd < now && rec.OrderStatus != OrderStatus.Оплачен).Include(rec=>rec.Client)
                .Select(rec => rec.Client).Distinct().ToListAsync();
            await StartPenetrating(clients);
            await context.SaveChangesAsync();
        }

        private Task StartPenetrating(List<Client> clients)
        {
            CountdownEvent countdown = new CountdownEvent(1);
            foreach (var client in clients)
            {
                countdown.AddCount();

                Task.Run(() =>
                {
                    client.Active = false;
                    countdown.Signal();
                });
            }
            countdown.Signal();

            countdown.Wait();
            return Task.Run(() => true);
        }

        public async Task RaiseBonuses(RetributionBindingModel model)
        {
            Client element = await context.Clients.FirstOrDefaultAsync(rec => rec.Id == model.ClientId);
            if (element == null)
            {
                throw new Exception("Элемент не найден");
            }
            element.Bonuses += model.Calculation;
            await context.SaveChangesAsync();
        }

        public async Task UpdElement(ClientBindingModel model)
        {
            Client element = await context.Clients.FirstOrDefaultAsync(rec =>
                                    (rec.ClientFIO == model.ClientFIO || rec.UserName == model.UserName) && rec.Id != model.Id);
            if (element != null)
            {
                throw new Exception("Уже есть клиент с такими данными");
            }
            element = await context.Clients.FirstOrDefaultAsync(rec => rec.Id == model.Id);
            if (element == null)
            {
                throw new Exception("Элемент не найден");
            }
            element.ClientFIO = model.ClientFIO;
            element.UserName = model.UserName;
            await context.SaveChangesAsync();
        }

        public async Task<AppUser> GetUser(int id)
        {
            Client element = await context.Clients.FirstOrDefaultAsync(rec => rec.Id == id);
            if (element != null)
            {
                return new AppUser
                {
                    Id = new AppId { Role = ApplicationRole.Client, Id = element.Id },
                    UserName = element.UserName,
                    PasswordHash = element.PasswordHash,
                    SecurityStamp = element.SecurityStamp
                };
            }
            return null;
        }

        public async Task<AppUser> GetUserByName(string name)
        {
            Client element = await context.Clients.FirstOrDefaultAsync(rec => rec.UserName.Equals(name));
            if (element != null)
            {
                return new AppUser
                {
                    Id = new AppId { Role = ApplicationRole.Client, Id = element.Id },
                    UserName = element.UserName,
                    PasswordHash = element.PasswordHash,
                    SecurityStamp = element.SecurityStamp
                };
            }
            return null;
        }
    }
}
