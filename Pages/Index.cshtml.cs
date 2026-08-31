using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentRecordDraft.Models;

namespace StudentRecordDraft.Pages;

public class IndexModel : PageModel
{
    // NOTE: Draft only. Students are stored in memory and reset on
    // restart. Entity Framework Core + a real database replaces this
    // once the full app is built.
    private static readonly List<Student> _students = new();
    private static int _nextId = 1;

    public List<Student> Students => _students;

    // Simple "algorithm" requirement satisfied here: WAM is just the
    // average of all recorded unit marks.
    public double Wam => _students.Count == 0 ? 0 : _students.Average(s => s.UnitMark);

    [BindProperty]
    public Student NewStudent { get; set; } = new();

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        NewStudent.Id = _nextId++;
        _students.Add(NewStudent);

        return RedirectToPage();
    }
}
