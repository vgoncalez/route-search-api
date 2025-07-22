using Microsoft.EntityFrameworkCore;
using RouteSearchApi.Domain.Models;

namespace RouteSearchApi.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<RouteModel> Rotas => Set<RouteModel>();

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RouteModel>().HasKey(r => r.Id);
            modelBuilder.Entity<RouteModel>().Property(r => r.Origem).IsRequired();
            modelBuilder.Entity<RouteModel>().Property(r => r.Destino).IsRequired();
            modelBuilder.Entity<RouteModel>().Property(r => r.Valor).IsRequired();
        }
    }
}
