namespace StudentRecordDraft.Models;

/// <summary>
/// A login account. Role is "Student" or "Admin". For the admin account
/// StudentId is null because the admin has no student record.
/// </summary>
public class Account
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;  // never store plain-text passwords
    public string Role { get; set; } = "Student";

    public int? StudentId { get; set; }                       // nullable value type: int?
    public Student? Student { get; set; }
}
