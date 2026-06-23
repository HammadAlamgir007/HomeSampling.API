namespace Module.Admin.Core.DBOs;

public class StatsDbo
{
    public int TotalPatients { get; set; }
    public int TotalAppointments { get; set; }
    public int PendingAppointments { get; set; }
    public int CompletedToday { get; set; }
    public int ActiveRiders { get; set; }
    public decimal RevenueThisMonth { get; set; }
}

public class AppointmentAdminDbo
{
    public int AppointmentId { get; set; }
    public int UserId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TestName { get; set; } = string.Empty;
    public int Status { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Address { get; set; } = string.Empty;
    public string? RiderName { get; set; }
    public int? RiderId { get; set; }
    public string? ReportFileName { get; set; }
    public int TotalCount { get; set; }
}

public class PatientAdminDbo
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int TotalBookings { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TestAdminDbo
{
    public int TestId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Duration { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RiderAdminDbo
{
    public int RiderId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int Status { get; set; }
    public double? LastLatitude { get; set; }
    public double? LastLongitude { get; set; }
    public int TotalTasksCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
}
