namespace StudentRecordDraft.Models;

public class Account
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;  // never store plain-text passwords
    public string Role { get; set; } = "Student";

    public int? StudentId { get; set; }                   
    public Student? Student { get; set; }
}
