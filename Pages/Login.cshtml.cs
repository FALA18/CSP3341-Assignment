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

public class LoginModel(AppDbContext db) : PageModel
{
    [BindProperty] public string Role { get; set; } = "Student";   
    [BindProperty] public string UserName { get; set; } = string.Empty;
    [BindProperty] public string Password { get; set; } = string.Empty;
    public string? Error { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        var account = await db.Accounts.FirstOrDefaultAsync(a => a.UserName == UserName);
        var hasher = new PasswordHasher<Account>();

        // The same message is used for "no such user" and "wrong password" 
        if (account is null || account.Role != Role ||          // the chosen role must match the account
            hasher.VerifyHashedPassword(account, account.PasswordHash, Password) == PasswordVerificationResult.Failed)
        {
            Error = "Invalid user name, password or role.";
            return Page();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, account.UserName),
            new(ClaimTypes.Role, account.Role)
        };
        if (account.StudentId is int studentId)               
            claims.Add(new Claim("StudentId", studentId.ToString()));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        return RedirectToPage("/Index");
    }
}
