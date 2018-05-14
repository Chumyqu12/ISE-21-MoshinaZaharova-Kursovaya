using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace VetService.App
{
    public class AppUserRole : IdentityUserRole<AppId> { }
    public class AppUserClaim : IdentityUserClaim<AppId> { }
    public class AppUserLogin : IdentityUserLogin<AppId> { }

    [DataContract]
    public class AppUser : IdentityUser<AppId, AppUserLogin, AppUserRole, AppUserClaim>
    {

        [DataMember]
        [Required]
        [Display(Name = "Имя пользователя")]
        public override string UserName { get; set; }

        [DataMember]
        public override AppId Id { get; set; }

        [DataMember]
        [Required]
        [StringLength(100, ErrorMessage = "Значение {0} должно содержать не менее {2} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public override string PasswordHash { get; set; }

        [DataMember]
        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("PasswordHash", ErrorMessage = "Пароль и его подтверждение не совпадают.")]
        public string ConfirmPassword { get; set; }

        [DataMember]
        public override string SecurityStamp { get; set; }

        [DataMember]
        public override ICollection<AppUserRole> Roles { get; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<AppUser, AppId> manager, string authenticationType)
        {
            var userIdentity = await manager.CreateIdentityAsync(
                this, authenticationType);
            return userIdentity;
        }
    }

    public class AppRole : IdentityRole<AppId, AppUserRole>
    {
        public AppRole() { }
        public AppRole(string name) { Name = name; }
    }
    public class AppRoleStore : RoleStore<AppRole, AppId, AppUserRole>
    {
        public AppRoleStore(AbstractDbContext context) : base(context) { }

    }

    public enum ApplicationRole
    {
        Admin = 0,

        Client = 1
    }
}
