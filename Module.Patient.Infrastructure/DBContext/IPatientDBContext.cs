using Module.Patient.Core.DBOs;

namespace Module.Patient.Infrastructure.DBContext;

public interface IPatientDBContext
{
    Task<List<TestDbo>> GetActiveTestsAsync();
    Task<AppointmentDbo?> GetAppointmentByDateAsync(int userId, int testId, DateTime date);
    Task<int> CreateBookingAsync(int userId, int testId, DateTime scheduledDate,
        string address, double? latitude, double? longitude);
    Task<List<AppointmentDbo>> GetPatientBookingsAsync(int userId);
    Task<AppointmentDbo?> GetAppointmentByIdAsync(int appointmentId, int userId);
    Task CancelBookingAsync(int appointmentId);
    Task<AppointmentDbo?> GetAppointmentByReportAsync(string reportFileName, int userId);
}
