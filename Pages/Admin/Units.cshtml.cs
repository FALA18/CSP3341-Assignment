using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentRecordDraft.Data;
using StudentRecordDraft.Models;

namespace StudentRecordDraft.Pages.Admin;

/// <summary>Lists units and lets the admin create a unit inside a course.</summary>
public class UnitsModel(AppDbContext db) : PageModel
{
    [BindProperty] public int CourseId { get; set; }
    [BindProperty] public string Code { get; set; } = string.Empty;
    [BindProperty] public string Name { get; set; } = string.Empty;
    public List<Unit> Units { get; set; } = [];
    public SelectList CourseList { get; set; } = new(Array.Empty<Course>());   // drop-down data

    public async Task OnGetAsync()
    {
        Units = await db.Units.Include(u => u.Course).OrderBy(u => u.UnitCode).ToListAsync();
        CourseList = new SelectList(await db.Courses.ToListAsync(), "Id", "CourseName");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (await db.Courses.AnyAsync(c => c.Id == CourseId) &&
            !string.IsNullOrWhiteSpace(Code) && !string.IsNullOrWhiteSpace(Name))
        {
            db.Units.Add(new Unit { CourseId = CourseId, UnitCode = Code.Trim(), UnitName = Name.Trim() });
            await db.SaveChangesAsync();
        }
        return RedirectToPage();
    }
}
