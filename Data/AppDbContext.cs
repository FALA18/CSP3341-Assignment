using Microsoft.EntityFrameworkCore;
using StudentRecordDraft.Models;

namespace StudentRecordDraft.Data;

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
    }
}
