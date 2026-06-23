namespace Module.Admin.Core.DTOs;

public class StatsDto
{
    public int TotalPatients { get; set; }
    public int TotalAppointments { get; set; }
    public int PendingAppointments { get; set; }
    public int CompletedToday { get; set; }
    public int ActiveRiders { get; set; }
    public decimal RevenueThisMonth { get; set; }
}

public class AppointmentAdminDto
{
    public int AppointmentId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TestName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public string Address { get; set; } = string.Empty;
    public string? RiderName { get; set; }
    public int? RiderId { get; set; }
    public string? ReportFileName { get; set; }
}

public class AppointmentPagedDto
{
    public List<AppointmentAdminDto> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PerPage { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PerPage);
}

public class UpdateStatusDto
{
    public int Status { get; set; }
}

public class BulkUpdateStatusDto
{
    public List<int> AppointmentIds { get; set; } = [];
    public int Status { get; set; }
}

public class AssignRiderDto
{
    public int RiderId { get; set; }
}

public class CreateTestDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Duration { get; set; }
}

public class CreateRiderDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class PatientAdminDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int TotalBookings { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TestAdminDto
{
    public int TestId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Duration { get; set; }
    public bool IsActive { get; set; }
}

public class RiderAdminDto
{
    public int RiderId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int TotalTasksCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class MessageDto
{
    public string Message { get; set; } = string.Empty;
}
