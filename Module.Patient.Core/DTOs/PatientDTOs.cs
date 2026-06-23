namespace Module.Patient.Core.DTOs;

public class TestDto
{
    public int TestId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Duration { get; set; }
}

public class BookingRequestDto
{
    public int TestId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Address { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

public class BookingDto
{
    public int AppointmentId { get; set; }
    public string TestName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public string Address { get; set; } = string.Empty;
    public string? ReportFileName { get; set; }
    public DateTime? ReportUploadedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class BookingCreatedDto
{
    public int AppointmentId { get; set; }
    public string Message { get; set; } = string.Empty;
}
