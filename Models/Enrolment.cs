namespace StudentRecordDraft.Models;

/// <summary>Links one student to one unit and stores the mark achieved.</summary>
public class Enrolment
{
    // Private backing field: the only way to change the mark is through the
    // property below, so an invalid mark can never be stored (encapsulation).
    private int _mark;

    public int Id { get; set; }
    public int StudentId { get; set; }
    public Student? Student { get; set; }
    public int UnitId { get; set; }
    public Unit? Unit { get; set; }
    public string Semester { get; set; } = string.Empty;

    /// <summary>Mark out of 100. The setter rejects values outside 0-100.</summary>
    public int Mark
    {
        get => _mark;
        set => _mark = value is >= 0 and <= 100               // pattern matching (relational pattern)
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value), "Mark must be between 0 and 100.");
    }
}
