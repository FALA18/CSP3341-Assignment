using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentRecordDraft.Data;
using StudentRecordDraft.Models;

namespace StudentRecordDraft.Pages.Admin;

/// <summary>Admin list and search of all student records.</summary>
public class IndexModel(AppDbContext db) : PageModel
{
    [BindProperty(SupportsGet = true)] public string? Search { get; set; }
    public List<Student> Students { get; set; } = [];

    public async Task OnGetAsync()
    {
        var query = db.Students.Include(s => s.Course).Include(s => s.Enrolments).AsQueryable();

        // LINQ query composition: the filter is only added when a search term exists,
        // and EF Core translates the whole expression into one SQL statement.
        if (!string.IsNullOrWhiteSpace(Search))
            query = query.Where(s => s.FullName.Contains(Search) || s.Email.Contains(Search));

        Students = await query.OrderBy(s => s.FullName).ToListAsync();
    }
}
