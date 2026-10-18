using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentRecordDraft.Data;
using StudentRecordDraft.Models;

namespace StudentRecordDraft.Pages.Admin;

/// <summary>Lists units and lets the admin create a unit inside a course or delete a unit.</summary>
public class UnitsModel(AppDbContext db) : PageModel
{
    [BindProperty] public int CourseId { get; set; }
    [BindProperty] public string Code { get; set; } = string.Empty;
    [BindProperty] public string Name { get; set; } = string.Empty;
    public List<Unit> Units { get; set; } = [];
    public SelectList CourseList { get; set; } = new(Array.Empty<Course>());   // drop-down data

    // Number of students enrolled in each unit, keyed by unit id (shown in the table).
    public Dictionary<int, int> EnrolmentCounts { get; set; } = new();

    // [TempData] keeps a message across the redirect that follows a successful post.
    [TempData] public string? Message { get; set; }
    [TempData] public string? Error { get; set; }

    public async Task OnGetAsync() => await LoadAsync();

    public async Task<IActionResult> OnPostAsync()
    {
        // Empty form fields bind as null, so ?? turns them into empty strings before trimming.
        var code = (Code ?? string.Empty).Trim();
        var name = (Name ?? string.Empty).Trim();

        if (!await db.Courses.AnyAsync(c => c.Id == CourseId))
            ModelState.AddModelError(nameof(CourseId), "Please choose a course (create one first if the list is empty).");
        if (code.Length == 0 || name.Length == 0)
            ModelState.AddModelError(string.Empty, "Please enter both a unit code and a unit name.");
        // Unit codes must be unique across all courses. ToUpper() makes the check
        // case-insensitive, so "dem1001" is treated as the same code as "DEM1001".
        else if (await db.Units.AnyAsync(u => u.UnitCode.ToUpper() == code.ToUpper()))
            ModelState.AddModelError(nameof(Code), $"A unit with the code \"{code}\" already exists.");

        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }   // redisplay the form with the error

        db.Units.Add(new Unit { CourseId = CourseId, UnitCode = code, UnitName = name });
        await db.SaveChangesAsync();
        Message = $"Unit {code} was added.";
        return RedirectToPage();
    }

    // Handler name "Delete" matches asp-page-handler="Delete" on each row's delete button.
    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var unit = await db.Units.FindAsync(id);
        if (unit is null) return RedirectToPage();

        // A unit that students are enrolled in cannot be deleted, because deleting it
        // would also delete their recorded marks.
        var enrolled = await db.Enrolments.CountAsync(e => e.UnitId == id);
        if (enrolled > 0)
        {
            Error = $"Unit {unit.UnitCode} cannot be deleted because {enrolled} student(s) are enrolled in it. Remove those enrolments first.";
            return RedirectToPage();
        }

        db.Units.Remove(unit);
        await db.SaveChangesAsync();
        Message = $"Unit {unit.UnitCode} was deleted.";
        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        Units = await db.Units.Include(u => u.Course).OrderBy(u => u.UnitCode).ToListAsync();
        CourseList = new SelectList(await db.Courses.ToListAsync(), "Id", "CourseName");

        // LINQ GroupBy: count enrolments per unit in a single query.
        EnrolmentCounts = await db.Enrolments
            .GroupBy(e => e.UnitId)
            .Select(g => new { UnitId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.UnitId, x => x.Count);
    }
}
