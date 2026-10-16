using System.Runtime.InteropServices;

namespace StudentRecordDraft.Models;

public class Student
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UnitMark { get; set; } = string.Empty;
    public string Status { get;set; } = "Active";

    public int CourseId { get;set; }
    public Course? Course { get;set; }
    public List<Enrolment> Enrolments { get;set; } = [];

    public double GetWam() =>
        Enrolments.Count == 0 ? 0 : Enrolments.Average(e => e.Mark);
}
 