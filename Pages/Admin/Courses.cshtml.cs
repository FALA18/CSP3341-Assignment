using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentRecordDraft.Data;
using StudentRecordDraft.Models;

namespace StudentRecordDraft.Pages.Admin;

/// <summary>Lists courses and lets the admin create or delete a course.</summary>
public class CoursesModel(AppDbContext db) : PageModel
{
    [BindProperty] public string Code { get; set; } = string.Empty;
    [BindProperty] public string Name { get; set; } = string.Empty;
    public List<Course> Courses { get; set; } = [];

    // [TempData] keeps a message across the redirect that follows a successful post.
    [TempData] public string? Message { get; set; }
    [TempData] public string? Error { get; set; }

    public async Task OnGetAsync() => await LoadAsync();

    public async Task<IActionResult> OnPostAsync()
    {
        // Empty form fields bind as null, so ?? turns them into empty strings before trimming.
        var code = (Code ?? string.Empty).Trim();
        var name = (Name ?? string.Empty).Trim();

        if (code.Length == 0 || name.Length == 0)
            ModelState.AddModelError(string.Empty, "Please enter both a course code and a course name.");
        // Course codes must be unique. ToUpper() makes the check case-insensitive,
        // so "bsc-cs" is treated as the same code as "BSc-CS".
        else if (await db.Courses.AnyAsync(c => c.CourseCode.ToUpper() == code.ToUpper()))
            ModelState.AddModelError(nameof(Code), $"A course with the code \"{code}\" already exists.");

        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }   // redisplay the form with the error

        db.Courses.Add(new Course { CourseCode = code, CourseName = name });
        await db.SaveChangesAsync();
        Message = $"Course {code} was added.";
        return RedirectToPage();                              // reload the list
    }

    // Handler name "Delete" matches asp-page-handler="Delete" on each row's delete button.
    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var course = await db.Courses
            .Include(c => c.Units)
            .Include(c => c.Students)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (course is null) return RedirectToPage();

        // A course that still has students cannot be deleted, because deleting it would
        // also delete those students' records.
        if (course.Students.Count > 0)
        {
            Error = $"Course {course.CourseCode} cannot be deleted because {course.Students.Count} student(s) are enrolled in it.";
            return RedirectToPage();
        }

        // Its units can only go with it if no student has a mark recorded against them.
        var unitIds = course.Units.Select(u => u.Id).ToList();
        if (await db.Enrolments.AnyAsync(e => unitIds.Contains(e.UnitId)))
        {
            Error = $"Course {course.CourseCode} cannot be deleted because its units have student enrolments.";
            return RedirectToPage();
        }

        db.Courses.Remove(course);                            // its units are deleted with it (cascade delete)
        await db.SaveChangesAsync();
        Message = $"Course {course.CourseCode} and its {unitIds.Count} unit(s) were deleted.";
        return RedirectToPage();
    }

    private async Task LoadAsync() =>
        Courses = await db.Courses
            .Include(c => c.Units)
            .Include(c => c.Students)
            .OrderBy(c => c.CourseCode)
            .ToListAsync();
}
