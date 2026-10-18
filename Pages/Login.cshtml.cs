using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentRecordDraft.Data;
using StudentRecordDraft.Models;

namespace StudentRecordDraft.Pages;

/// <summary>Signs a user in after checking user name, chosen role and password hash.</summary>
public class LoginModel(AppDbContext db) : PageModel
{
    [BindProperty] public string Role { get; set; } = "Student";   // chosen on the login page
    [BindProperty] public string UserName { get; set; } = string.Empty;
    [BindProperty] public string Password { get; set; } = string.Empty;
    public string? Error { get; set; }

    // async/await: the thread is released while the database query runs.
    public async Task<IActionResult> OnPostAsync()
    {
        var account = await db.Accounts.FirstOrDefaultAsync(a => a.UserName == UserName);
        var hasher = new PasswordHasher<Account>();

        // The same message is used for "no such user" and "wrong password" (no user enumeration).
        if (account is null || account.Role != Role ||          // the chosen role must match the account
            hasher.VerifyHashedPassword(account, account.PasswordHash, Password) == PasswordVerificationResult.Failed)
        {
            Error = "Invalid user name, password or role.";
            return Page();
        }

        // Claims describe the logged-in user; the Role claim drives access control.
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, account.UserName),
            new(ClaimTypes.Role, account.Role)
        };
        if (account.StudentId is int studentId)               // pattern matching on int?
            claims.Add(new Claim("StudentId", studentId.ToString()));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        return RedirectToPage("/Index");                     // Index sends each role to its home page
    }
}
