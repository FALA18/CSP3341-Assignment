namespace StudentRecordDraft.Models;

public class Unit
{
    public int Id { get; set; }
    public string UnitCode { get; set; } = string.Empty;
    public string UnitName { get; set; } = string.Empty;

    public int CourseId { get; set; }                         
    public Course? Course { get; set; }                       
}
