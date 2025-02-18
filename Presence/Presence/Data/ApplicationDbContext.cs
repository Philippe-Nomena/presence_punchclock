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
        public DbSet<Postal>Postals  { get; set; } = null;
        public DbSet<Employe> Employes  { get; set; } = null;
        public DbSet<Organisation> Organisations { get; set; } = null;
        public DbSet<Departement> Departements { get; set; }
        public DbSet<Shift> Shifts { get; set; } = null;
        public DbSet<Present> Presents { get; set; } = null;


    }
}
