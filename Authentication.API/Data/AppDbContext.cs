using Authentication.API.Infrastructure.Configurations.Entity;
using Authentication.API.Infrastructure.Configurations.Mappings;
using Microsoft.EntityFrameworkCore;

namespace Authentication.API.Data
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new IntegrationRefreshTokenMapping());
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<Entity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedOn = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedOn = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Deleted)
                {
                    entry.Entity.DeletedOn = DateTime.UtcNow;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        public void HealthCheck()
        {
            Database.OpenConnection();
            Database.CloseConnection();
        }

    }
}