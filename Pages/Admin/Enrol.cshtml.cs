using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentRecordDraft.Data;
using StudentRecordDraft.Models;

namespace StudentRecordDraft.Pages.Admin;

/// <summary>Shows a student's units and lets the admin add or remove an enrolment.</summary>
public class EnrolModel(AppDbContext db) : PageModel
{
    public Student? Student { get; set; }
    public SelectList UnitList { get; set; } = new(Array.Empty<Unit>());

    [BindProperty] public int UnitId { get; set; }
    [BindProperty] public string Semester { get; set; } = "2026-S2";
    [BindProperty, Range(0, 100)] public int Mark { get; set; }

    public async Task<IActionResult> OnGetAsync(int id) => await LoadAsync(id) ? Page() : NotFound();

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!await LoadAsync(id)) return NotFound();
        if (UnitId == 0) ModelState.AddModelError("", "No unit available. Create a unit for this course first.");
        if (!ModelState.IsValid) return Page();

        db.Enrolments.Add(new Enrolment { StudentId = id, UnitId = UnitId, Semester = Semester, Mark = Mark });
        await db.SaveChangesAsync();
        return RedirectToPage(new { id });
    }
    public async Task<IActionResult> OnPostRemoveAsync(int id, int enrolmentId)
    {
        var enrolment = await db.Enrolments.FindAsync(enrolmentId);
        if (enrolment is not null)
        {
            db.Enrolments.Remove(enrolment);
            await db.SaveChangesAsync();
        }
        return RedirectToPage(new { id });
    }

    private async Task<bool> LoadAsync(int id)
    {
        Student = await db.Students.Include(s => s.Enrolments).ThenInclude(e => e.Unit)
                                   .FirstOrDefaultAsync(s => s.Id == id);
        if (Student is null) return false;

        // LINQ: offer only units from the student's course that they are not already taking.
        var taken = Student.Enrolments.Select(e => e.UnitId).ToList();
        var available = await db.Units
            .Where(u => u.CourseId == Student.CourseId && !taken.Contains(u.Id))
            .ToListAsync();
        UnitList = new SelectList(available, "Id", "UnitName");
        return true;
    }
}
