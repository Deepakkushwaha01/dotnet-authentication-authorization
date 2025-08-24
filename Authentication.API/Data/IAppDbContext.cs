namespace Authentication.API.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;

    public interface IAppDbContext : IDisposable
    {
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        ChangeTracker ChangeTracker { get; }

        void HealthCheck();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}