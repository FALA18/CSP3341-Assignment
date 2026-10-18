using Microsoft.EntityFrameworkCore;
using StudentRecordDraft.Models;

namespace StudentRecordDraft.Data;

/// <summary>Entity Framework Core database context (maps classes to SQLite tables).</summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<Enrolment> Enrolments => Set<Enrolment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User names must be unique so a login identifies exactly one account.
        modelBuilder.Entity<Account>().HasIndex(a => a.UserName).IsUnique();

        // The database also refuses duplicate course codes, unit codes and student names,
        // as a second line of defence behind the checks on the admin pages.
        modelBuilder.Entity<Course>().HasIndex(c => c.CourseCode).IsUnique();
        modelBuilder.Entity<Unit>().HasIndex(u => u.UnitCode).IsUnique();
        modelBuilder.Entity<Student>().HasIndex(s => s.FullName).IsUnique();
    }
}
