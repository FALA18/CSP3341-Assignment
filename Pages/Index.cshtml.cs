using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudentRecordDraft.Pages;

/// <summary>Home page: sends each visitor to the page that matches their role.</summary>
public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        if (User.IsInRole("Admin")) return RedirectToPage("/Admin/Index");
        if (User.IsInRole("Student")) return RedirectToPage("/MyRecord");
        return RedirectToPage("/Login");
    }
}
