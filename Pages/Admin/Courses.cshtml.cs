using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentRecordDraft.Data;
using StudentRecordDraft.Models;

namespace StudentRecordDraft.Pages.Admin;

/// <summary>Lists courses and lets the admin create a new one.</summary>
public class CoursesModel(AppDbContext db) : PageModel
{
    [BindProperty] public string Code { get; set; } = string.Empty;
    [BindProperty] public string Name { get; set; } = string.Empty;
    public List<Course> Courses { get; set; } = [];

    public async Task OnGetAsync() =>
        Courses = await db.Courses.Include(c => c.Units).OrderBy(c => c.CourseCode).ToListAsync();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!string.IsNullOrWhiteSpace(Code) && !string.IsNullOrWhiteSpace(Name))
        {
            db.Courses.Add(new Course { CourseCode = Code.Trim(), CourseName = Name.Trim() });
            await db.SaveChangesAsync();
        }
        return RedirectToPage();                              // reload the list
    }
}
