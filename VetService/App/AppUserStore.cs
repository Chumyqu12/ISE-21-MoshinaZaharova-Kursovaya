using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetService.VetBindingModels;
using VetService.VetInterfaces;

namespace VetService.App
{
    public class AppUserStore : UserStore<AppUser, AppRole, AppId, AppUserLogin, AppUserRole, AppUserClaim>
    {
        private readonly IAdminService serviceA;

        private readonly IClientService serviceC;

        public AppUserStore(AbstractDbContext context, IAdminService serviceA, IClientService serviceC) : base(context)
        {
            this.serviceA = serviceA;
            this.serviceC = serviceC;
        }

        public override Task<AppUser> FindByIdAsync(AppId userId)
        {
            if (userId.Role.Equals(ApplicationRole.Admin))
            {
                return serviceA.GetUser(userId.Id);
            }
            else if (userId.Role.Equals(ApplicationRole.Client))
            {
                return serviceC.GetUser(userId.Id);
            }
            return null;
        }
        public override Task CreateAsync(AppUser user)
        {

            if (user is AdminCreateModel)
            {
                return serviceA.AddElement(user as AdminCreateModel);
            }
            else if (user is ClientCreateModel)
            {
                return serviceC.AddElement(user as ClientCreateModel);
            }
            return null;
        }

        public override Task DeleteAsync(AppUser user)
        {
            if (user.Id.Role.Equals(ApplicationRole.Admin))
            {
                return serviceA.DelElement(user.Id.Id);
            }else if (user.Id.Role.Equals(ApplicationRole.Client))
            {
                return serviceC.DelElement(user.Id.Id);
            }

            return null;
        }

        public async override Task<AppUser> FindByNameAsync(string userName)
        {
            AppUser user = null;

            if ((user = await serviceC.GetUserByName(userName)) != null)
            {
                return user;
            }
            if ((user = await serviceA.GetUserByName(userName)) != null)
            {
                return user;
            }
            return null;
        }
    }
}
