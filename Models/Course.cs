namespace StudentRecordDraft.Models;

/// <summary>A course (degree) made up of several units.</summary>
public class Course
{
    public int Id { get; set; }
    public string CourseCode { get; set; } = string.Empty;   // "= string.Empty" keeps the compiler's null-safety happy
    public string CourseName { get; set; } = string.Empty;

    public List<Unit> Units { get; set; } = [];               // one course -> many units
    public List<Student> Students { get; set; } = [];         // one course -> many students
}
