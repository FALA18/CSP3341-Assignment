using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentRecordDraft.Data;
using StudentRecordDraft.Models;

namespace StudentRecordDraft.Pages.Admin;

/// <summary>Admin page for editing a student's details, status and unit marks.</summary>
public class EditModel(AppDbContext db) : PageModel
{
    [BindProperty] public EditInput Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var student = await db.Students
            .Include(s => s.Enrolments).ThenInclude(e => e.Unit)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (student is null) return NotFound();

        // LINQ Select projects each enrolment into the form's input object.
        Input = new EditInput
        {
            FullName = student.FullName,
            Email = student.Email,
            Status = student.Status,
            Marks = student.Enrolments
                .Select(e => new MarkInput { EnrolmentId = e.Id, UnitName = e.Unit?.UnitName, Mark = e.Mark })
                .ToList()
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!StudentStatus.IsValid(Input.Status))
            ModelState.AddModelError("Input.Status", "Please choose a valid status.");

        // The new name must not belong to a different student (case-insensitive check).
        var fullName = (Input.FullName ?? string.Empty).Trim();
        if (fullName.Length > 0 &&
            await db.Students.AnyAsync(s => s.Id != id && s.FullName.ToUpper() == fullName.ToUpper()))
            ModelState.AddModelError("Input.FullName", $"Another student named \"{fullName}\" already exists.");
        if (!ModelState.IsValid) return Page();               // show validation errors

        var student = await db.Students.Include(s => s.Enrolments).FirstOrDefaultAsync(s => s.Id == id);
        if (student is null) return NotFound();

        student.FullName = fullName;
        student.Email = Input.Email;
        student.Status = Input.Status;
        foreach (var m in Input.Marks)
        {
            var enrolment = student.Enrolments.FirstOrDefault(e => e.Id == m.EnrolmentId);
            if (enrolment is not null) enrolment.Mark = m.Mark;   // setter re-checks the 0-100 rule
        }
        await db.SaveChangesAsync();
        return RedirectToPage("./Index");
    }
}

/// <summary>Form fields for the student (data annotations give server-side validation).</summary>
public class EditInput
{
    [Required, StringLength(100)] public string FullName { get; set; } = string.Empty;
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] public string Status { get; set; } = StudentStatus.Active;
    public List<MarkInput> Marks { get; set; } = [];
}

public class MarkInput
{
    public int EnrolmentId { get; set; }
    public string? UnitName { get; set; }
    [Range(0, 100)] public int Mark { get; set; }
}
