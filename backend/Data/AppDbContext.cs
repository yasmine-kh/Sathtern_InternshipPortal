using backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Internship> Internships => Set<Internship>();
    public DbSet<Application> Applications => Set<Application>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasIndex(s => s.Email).IsUnique();
        });

        modelBuilder.Entity<Application>(entity =>
        {
            // One application per student per internship.
            entity.HasIndex(a => new { a.StudentId, a.InternshipId }).IsUnique();

            entity.Property(a => a.Status)
                  .HasConversion<string>()
                  .HasMaxLength(20);

            entity.HasOne(a => a.Student)
                  .WithMany(s => s.Applications)
                  .HasForeignKey(a => a.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(a => a.Internship)
                  .WithMany(i => i.Applications)
                  .HasForeignKey(a => a.InternshipId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
