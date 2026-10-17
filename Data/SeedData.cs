using Microsoft.AspNetCore.Identity;
using StudentRecordDraft.Models;

namespace StudentRecordDraft.Data;

/// <summary>Inserts demo data the first time the database is created.</summary>
public static class SeedData
{
    public static void Initialise(AppDbContext db)
    {
        if (db.Accounts.Any()) return;                        // already seeded

        var course = new Course { CourseCode = "BSc-CS", CourseName = "Bachelor of Science (Computer Science)" };
        var units = new[]
        {
            new Unit { UnitCode = "DEM1001", UnitName = "Introduction to Programming", Course = course },
            new Unit { UnitCode = "DEM1002", UnitName = "Databases", Course = course },
            new Unit { UnitCode = "DEM2001", UnitName = "Web Development", Course = course }
        };

        // Local helper: builds an enrolment for a student in a unit.
        Enrolment Enrol(Unit u, int mark) => new() { Unit = u, Mark = mark, Semester = "2026-S2" };

        var alice = new Student
        {
            FullName = "Alice Perera", Email = "alice@example.com", Course = course,
            Enrolments = [Enrol(units[0], 78), Enrol(units[1], 85), Enrol(units[2], 64)]
        };
        var nuwan = new Student
        {
            FullName = "Nuwan Silva", Email = "nuwan@example.com", Course = course,
            Enrolments = [Enrol(units[0], 55), Enrol(units[1], 62), Enrol(units[2], 71)]
        };
        db.Students.AddRange(alice, nuwan);
        db.SaveChanges();                                     // generates the student Ids

        // Passwords are hashed with ASP.NET Core's PasswordHasher (salted, slow hash).
        var hasher = new PasswordHasher<Account>();
        Account Make(string user, string password, string role, int? studentId = null)
        {
            var account = new Account { UserName = user, Role = role, StudentId = studentId };
            account.PasswordHash = hasher.HashPassword(account, password);
            return account;
        }

        db.Accounts.AddRange(
            Make("admin", "Admin#123", "Admin"),
            Make("alice", "Student#123", "Student", alice.Id),
            Make("nuwan", "Student#123", "Student", nuwan.Id));
        db.SaveChanges();
    }
}
