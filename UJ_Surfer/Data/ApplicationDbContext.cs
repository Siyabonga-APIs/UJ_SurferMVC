using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UJ_Surfer.Models;

namespace UJ_Surfer.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<StudentMatricProfile> StudentMatricProfiles => Set<StudentMatricProfile>();
        public DbSet<Faculty> Faculties => Set<Faculty>();
        public DbSet<Contact> Contacts => Set<Contact>();
        public DbSet<Programme> Programmes => Set<Programme>();
        public DbSet<SubjectRequirement> SubjectRequirements => Set<SubjectRequirement>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Critical for Identity mapping

            // Faculty -> Contact (Cascade delete)
            builder.Entity<Faculty>()
                .HasMany(f => f.Contacts)
                .WithOne(c => c.Faculty)
                .HasForeignKey(c => c.FacultyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Faculty -> Programme (Cascade delete)
            builder.Entity<Faculty>()
                .HasMany(f => f.Programmes)
                .WithOne(p => p.Faculty)
                .HasForeignKey(p => p.FacultyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Programme -> SubjectRequirement (Cascade delete)
            builder.Entity<Programme>()
                .HasMany(p => p.SubjectRequirements)
                .WithOne(s => s.Programme)
                .HasForeignKey(s => s.ProgrammeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
