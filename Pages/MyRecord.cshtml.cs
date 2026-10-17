using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentRecordDraft.Data;
using StudentRecordDraft.Models;

namespace StudentRecordDraft.Pages;

/// <summary>Shows the loggedin student's own record </summary>
public class MyRecordModel(AppDbContext db) : PageModel
{
    public Student? Student { get; set; }
    public double Wam { get; set; }

    public async Task OnGetAsync()
    {
        var claim = User.FindFirst("StudentId");
        if (claim is null || !int.TryParse(claim.Value, out var id)) return;

        Student = await db.Students
            .Include(s => s.Course)
            .Include(s => s.Enrolments).ThenInclude(e => e.Unit)  
            .FirstOrDefaultAsync(s => s.Id == id);

        Wam = Student?.GetWam() ?? 0;                              
    }
}
