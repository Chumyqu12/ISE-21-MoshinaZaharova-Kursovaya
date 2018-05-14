using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetModel;
using VetService.App;
using VetService.VetBindingModels;
using VetService.VetInterfaces;
using VetService.VetViewModels;

namespace VetService.VetImplementations
{
    public class AdminService : IAdminService
    {
        private AbstractDbContext context;

        public AdminService(AbstractDbContext context)
        {
            this.context = context;
        }

        public static AdminService Create(AbstractDbContext context)
        {
            return new AdminService(context);
        }

        public async Task AddElement(AdminCreateModel model)
        {
            Admin element = await context.Admins.FirstOrDefaultAsync(rec => rec.AdminFIO == model.AdminFIO);
            if (element != null)
            {
                throw new Exception("Already have a admin with such a name");
            }
            element = await context.Admins.FirstOrDefaultAsync(rec => rec.UserName == model.UserName);
            if (element != null)
            {
                throw new Exception("Already have a admin with such a user name");
            }
            context.Admins.Add(new Admin
            {
                AdminFIO = model.AdminFIO,
                UserName = model.UserName,
                PasswordHash = model.PasswordHash,
                SecurityStamp = model.SecurityStamp
            });
            await context.SaveChangesAsync();
        }

        public Task BackUp()
        {
            throw new NotImplementedException();
        }

        public async Task DelElement(int id)
        {
            Admin element = await context.Admins.FirstOrDefaultAsync(rec => rec.Id == id);
            if (element != null)
            {
                context.Admins.Remove(element);
                await context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Элемент не найден");
            }
        }

        public async Task<AdminViewModel> GetElement(int id)
        {
            Admin element = await context.Admins.FirstOrDefaultAsync(rec => rec.Id == id);
            if (element != null)
            {
                return new AdminViewModel
                {
                    Id = element.Id,
                    AdminFIO = element.AdminFIO,
                    UserName = element.UserName
                };
            }
            throw new Exception("Element not found");
        }

        public async Task<List<AdminViewModel>> GetList()
        {
            List<AdminViewModel> result = await context.Admins.Select(rec => new AdminViewModel
            {
                Id = rec.Id,
                AdminFIO = rec.AdminFIO,
                UserName = rec.UserName
            })
                .ToListAsync();
            return result;
        }

        public async Task<AppUser> GetUser(int id)
        {
            Admin element = await context.Admins.FirstOrDefaultAsync(rec => rec.Id == id);
            if (element != null)
            {
                return new AppUser
                {
                    Id = new AppId { Role = ApplicationRole.Admin, Id = element.Id },
                    UserName = element.UserName,
                    PasswordHash = element.PasswordHash,
                    SecurityStamp = element.SecurityStamp
                };
            }
            return null;
        }

        public async Task<AppUser> GetUserByName(string name)
        {
            Admin element = await context.Admins.FirstOrDefaultAsync(rec => rec.UserName.Equals(name));
            if (element != null)
            {
                return new AppUser
                {
                    Id = new AppId { Role = ApplicationRole.Admin, Id = element.Id },
                    UserName = element.UserName,
                    PasswordHash = element.PasswordHash,
                    SecurityStamp = element.SecurityStamp
                };
            }
            return null;
        }

        public async Task UpdElement(AdminBindingModel model)
        {
            Admin element = await context.Admins.FirstOrDefaultAsync(rec =>
                                    (rec.AdminFIO == model.AdminFIO || rec.UserName == rec.UserName) && rec.Id != model.Id);
            if (element != null)
            {
                throw new Exception("Уже есть админ с такими данными");
            }
            element = await context.Admins.FirstOrDefaultAsync(rec => rec.Id == model.Id);
            if (element == null)
            {
                throw new Exception("Элемент не найден");
            }
            element.AdminFIO = model.AdminFIO;
            element.UserName = model.UserName;
            await context.SaveChangesAsync();
        }

    }
}
