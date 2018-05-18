using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using VetRestApi.Models;
using VetService;
using VetService.App;
using VetService.VetImplementations;

namespace VetRestApi
{
    // Настройка диспетчера пользователей приложения. UserManager определяется в ASP.NET Identity и используется приложением.

    public class ApplicationUserManager : UserManager<AppUser,AppId>
    {
        public ApplicationUserManager(IUserStore<AppUser,AppId> store)
            : base(store)
        {
        }

        public override async Task<ClaimsIdentity> CreateIdentityAsync(AppUser user, string authenticationType)
        {
            IList<Claim> claimCollection = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Id.Role.ToString()),
            };
            var claimsIdentity = new ClaimsIdentity(claimCollection, authenticationType);

            return await Task.Run(() => claimsIdentity);
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new AppUserStore(context.Get<AbstractDbContext>(),AdminService.Create(context.Get<AbstractDbContext>()), ClientService.Create(context.Get<AbstractDbContext>())));
            // Настройка логики проверки имен пользователей
            manager.UserValidator = new UserValidator<AppUser,AppId>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false
            };
            // Настройка логики проверки паролей
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<AppUser,AppId>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }
}
