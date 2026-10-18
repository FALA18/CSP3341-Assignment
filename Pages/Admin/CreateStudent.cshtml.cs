using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentRecordDraft.Data;
using StudentRecordDraft.Models;

namespace StudentRecordDraft.Pages.Admin;

/// <summary>Creates a student record and the student's login account in one step.</summary>
public class CreateStudentModel(AppDbContext db) : PageModel
{
    [BindProperty] public StudentInput Input { get; set; } = new();
    public SelectList CourseList { get; set; } = new(Array.Empty<Course>());

    public async Task OnGetAsync() => await LoadCoursesAsync();

    public async Task<IActionResult> OnPostAsync()
    {
        if (await db.Accounts.AnyAsync(a => a.UserName == Input.UserName))
            ModelState.AddModelError("Input.UserName", "That user name is already taken.");

        // Student names must be unique. The check ignores case and surrounding spaces,
        // so "alice perera" is treated as the same name as "Alice Perera".
        var fullName = (Input.FullName ?? string.Empty).Trim();
        if (fullName.Length > 0 && await db.Students.AnyAsync(s => s.FullName.ToUpper() == fullName.ToUpper()))
            ModelState.AddModelError("Input.FullName", $"A student named \"{fullName}\" already exists.");
        if (!ModelState.IsValid) { await LoadCoursesAsync(); return Page(); }

        var student = new Student { FullName = fullName, Email = Input.Email, CourseId = Input.CourseId };
        db.Students.Add(student);
        await db.SaveChangesAsync();                          // generates student.Id

        var account = new Account { UserName = Input.UserName, Role = "Student", StudentId = student.Id };
        account.PasswordHash = new PasswordHasher<Account>().HashPassword(account, Input.Password);
        db.Accounts.Add(account);
        await db.SaveChangesAsync();
        return RedirectToPage("./Index");
    }

    private async Task LoadCoursesAsync() =>
        CourseList = new SelectList(await db.Courses.ToListAsync(), "Id", "CourseName");
}

public class StudentInput
{
    [Required, StringLength(100)] public string FullName { get; set; } = string.Empty;
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Range(1, int.MaxValue, ErrorMessage = "Please choose a course (create one first if the list is empty).")]
    public int CourseId { get; set; }
    [Required] public string UserName { get; set; } = string.Empty;
    [Required, MinLength(6)] public string Password { get; set; } = string.Empty;
}
