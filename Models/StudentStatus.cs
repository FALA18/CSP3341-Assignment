namespace StudentRecordDraft.Models;

/// <summary>
/// The allowed values for <see cref="Student.Status"/>, kept in one place so the
/// edit form, the validation and the status badges all agree.
/// </summary>
public static class StudentStatus
{
    public const string Active = "Active";
    public const string OnLeave = "On leave";
    public const string Suspended = "Suspended";
    public const string Withdrawn = "Withdrawn";
    public const string Graduated = "Graduated";

    public static readonly IReadOnlyList<string> All = [Active, OnLeave, Suspended, Withdrawn, Graduated];

    public static bool IsValid(string? status) => status is not null && All.Contains(status);

    /// <summary>CSS class for the coloured status badge (switch expression).</summary>
    public static string BadgeClass(string? status) => status switch
    {
        Active => "status-active",
        OnLeave => "status-leave",
        Suspended => "status-suspended",
        Withdrawn => "status-withdrawn",
        Graduated => "status-graduated",
        _ => "status-other"
    };
}
