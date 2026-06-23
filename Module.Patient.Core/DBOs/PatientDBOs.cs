namespace Module.Patient.Core.DBOs;

public class TestDbo
{
    public int TestId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Duration { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AppointmentDbo
{
    public int AppointmentId { get; set; }
    public int UserId { get; set; }
    public int TestId { get; set; }
    public string TestName { get; set; } = string.Empty;
    public int? RiderId { get; set; }
    public int Status { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Address { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? ReportFileName { get; set; }
    public DateTime? ReportUploadedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
