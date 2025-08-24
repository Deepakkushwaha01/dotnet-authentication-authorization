using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Authentication.API.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public void HealthCheck()
        {
            Database.OpenConnection();
            Database.CloseConnection();
        }

    }
}