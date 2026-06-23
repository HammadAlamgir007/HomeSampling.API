namespace Module.Rider.Core.DBOs;

public class RiderDbo
{
    public int RiderId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? PhotoPath { get; set; }
    public int Status { get; set; }
    public double? LastLatitude { get; set; }
    public double? LastLongitude { get; set; }
    public DateTime? LastLocationAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TaskDbo
{
    public int AppointmentId { get; set; }
    public int UserId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string PatientPhone { get; set; } = string.Empty;
    public string TestName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int Status { get; set; }
    public DateTime ScheduledDate { get; set; }
}

public class TaskLogDbo
{
    public int LogId { get; set; }
    public int AppointmentId { get; set; }
    public int RiderId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string? PhotoPath { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class NotificationDbo
{
    public int NotificationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}
