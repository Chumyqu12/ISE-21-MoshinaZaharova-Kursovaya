using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Threading.Tasks;
using VetModel;

namespace VetService
{
    public class AbstractDbContext : DbContext
    {
        public AbstractDbContext() : base("AbstractVet")
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
            var ensureDLLIsCopied = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }

        public static AbstractDbContext Create()
        {
            return new AbstractDbContext();
        }

        public override Task<int> SaveChangesAsync()
        {
            try
            {
                return base.SaveChangesAsync();
            }
            catch (Exception)
            {
                foreach (var entry in ChangeTracker.Entries())
                {
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            entry.State = EntityState.Unchanged;
                            break;
                        case EntityState.Deleted:
                            entry.Reload();
                            break;
                        case EntityState.Added:
                            entry.State = EntityState.Detached;
                            break;
                    }
                }
                throw;
            }
        }
        public virtual DbSet<Admin> Admins { get; set; }

        public virtual DbSet<Client> Clients { get; set; }

        public virtual DbSet<Pay> Pays { get; set; }

        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<ServiceOrder> ServiceOrders { get; set; }

        public virtual DbSet<Service> Services { get; set; }
    }
}
