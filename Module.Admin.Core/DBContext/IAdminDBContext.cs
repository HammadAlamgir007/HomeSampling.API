using Module.Admin.Core.DBOs;

namespace Module.Admin.Core.DBContext;

public interface IAdminDBContext
{
    Task<StatsDbo?> GetDashboardStatsAsync();
    Task<List<AppointmentAdminDbo>> GetAppointmentsAsync(int page, int perPage, string? search);
    Task UpdateAppointmentStatusAsync(int appointmentId, int status);
    Task BulkUpdateStatusAsync(List<int> appointmentIds, int status);
    Task AssignRiderAsync(int appointmentId, int riderId);
    Task<int?> AutoAssignRiderAsync(int appointmentId);
    Task SaveReportPathAsync(int appointmentId, string reportFileName);
    Task<List<PatientAdminDbo>> GetAllPatientsAsync();
    Task<List<TestAdminDbo>> GetAllTestsAsync();
    Task CreateTestAsync(string name, string description, decimal price, int duration);
    Task UpdateTestAsync(int testId, string name, string description, decimal price, int duration);
    Task DeleteTestAsync(int testId);
    Task<List<RiderAdminDbo>> GetAllRidersAsync();
    Task<RiderAdminDbo?> GetRiderByIdAsync(int riderId);
    Task CreateRiderAsync(string name, string email, string phone, string passwordHash);
    Task DeleteRiderAsync(int riderId);
}
