namespace Shared.Infrastructure.Enums;

public enum UserRole
{
    Patient,
    Admin,
    Rider
}

public enum AppointmentStatus
{
    Pending = 0,
    Confirmed = 1,
    AssignedRider = 2,
    OnWay = 3,
    Arrived = 4,
    Collected = 5,
    Delivered = 6,
    ReportUploaded = 7,
    Cancelled = 8
}

public enum RiderStatus
{
    Available,
    Busy,
    Offline
}

public enum OtpPurpose
{
    Register,
    ResetPassword
}
