namespace StudentRecordDraft.Models;

/// <summary>Personal and academic details of one student.</summary>
public class Student
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = StudentStatus.Active; // current enrolment status (see StudentStatus)

    public int CourseId { get; set; }
    public Course? Course { get; set; }
    public List<Enrolment> Enrolments { get; set; } = [];

    /// <summary>
    /// Weighted Average Mark: the average of all unit marks, calculated with LINQ.
    /// Average() returns a double, so there is no integer truncation.
    /// NOTE: Enrolments must be loaded (Include) before calling this method.
    /// </summary>
    public double GetWam() =>
        Enrolments.Count == 0 ? 0 : Enrolments.Average(e => e.Mark);
}
