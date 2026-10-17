namespace StudentRecordDraft.Models;

/// <summary>A single unit (subject) that belongs to a course.</summary>
public class Unit
{
    public int Id { get; set; }
    public string UnitCode { get; set; } = string.Empty;
    public string UnitName { get; set; } = string.Empty;

    public int CourseId { get; set; }                         // foreign key
    public Course? Course { get; set; }                       // navigation property
}
