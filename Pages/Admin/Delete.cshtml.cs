using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentRecordDraft.Data;
using StudentRecordDraft.Models;

namespace StudentRecordDraft.Pages.Admin;

/// <summary>Confirmation page and handler for deleting a student record.</summary>
public class DeleteModel(AppDbContext db) : PageModel
{
    public Student? Student { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Student = await db.Students.FindAsync(id);
        return Student is null ? NotFound() : Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var student = await db.Students.FindAsync(id);
        if (student is null) return NotFound();

        // Remove the student's login account, then the student (enrolments cascade-delete).
        db.Accounts.RemoveRange(db.Accounts.Where(a => a.StudentId == id));
        db.Students.Remove(student);
        await db.SaveChangesAsync();
        return RedirectToPage("./Index");
    }
}
