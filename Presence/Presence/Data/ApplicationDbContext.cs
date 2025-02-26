using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Presence.Models;

namespace Presence.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Postal> Postals { get; set; } = default!;
        public DbSet<Employe> Employes { get; set; } = default!;
        public DbSet<Organisation> Organisations { get; set; } = default!;
        public DbSet<Departement> Departements { get; set; } = default!;
        public DbSet<Shift> Shifts { get; set; } = default!;
        public DbSet<Present> Presents { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

          
            modelBuilder.Entity<Present>()
                .HasOne(p => p.Employe)
                .WithMany(e => e.Presences)
                .HasForeignKey(p => p.IdEmploye)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Present>()
                .HasOne(p => p.Departement)
                .WithMany()
                .HasForeignKey(p => p.IdDepartement)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Present>()
                .HasOne(p => p.Shift)
                .WithMany()
                .HasForeignKey(p => p.IdShift)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
