using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Module.Patient.Core.DBContext;
using Module.Patient.Core.DBOs;
using Shared.Infrastructure.DBContext;
using Shared.Infrastructure.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Module.Patient.Infrastructure.DBContext;

public class PatientDBContext : BaseDBContext, IPatientDBContext
{
    public PatientDBContext(IOptions<DatabaseConnection> options) : base(options) { }

    public async Task<List<TestDbo>> GetActiveTestsAsync()
    {
        return await QueryListAsync<TestDbo>("sp_GetActiveTests");
    }
    public async Task<AppointmentDbo?> GetAppointmentByDateAsync(
        int userId, int testId, DateTime date)
    {
        return await QuerySingleAsync<AppointmentDbo>(
            "sp_GetAppointmentByDate",
            new
            {
                UserId= userId,
                TestId= testId,
                Date=date.Date
            });
    }
       

    public async Task<int> CreateBookingAsync(
        int userId, int testId, DateTime scheduledDate,
        string address, double? latitude, double? longitude)
    {
        return await ExecuteScalarAsync<int>(
              "sp_CreateBooking",
              new
              {
                  UserId = userId,
                  TestId = testId,
                  ScheduledDate = scheduledDate,
                  Address = address,
                  Latitude = latitude,
                  Longitude = longitude
              });
    }

    public async Task<List<AppointmentDbo>> GetPatientBookingsAsync(int userId)
    {

        return await QueryListAsync<AppointmentDbo>("sp_GetPatientBookings",
            new
            {
                UserId = userId,
            });
    }
    public async Task<AppointmentDbo?> GetAppointmentByIdAsync(
        int appointmentId, int userId)
    {
        return await QuerySingleAsync<AppointmentDbo>("sp_GetAppointmentById",
             new
             {
                 AppoitmentId = appointmentId,
                 UserID = userId,

             });
        
    }

    public async Task CancelBookingAsync(int appointmentId)
    {

        await ExecuteAsync("sp_CancelBooking",
            new
            {
                AppoitmentId=appointmentId
            });
    }

    public async Task<AppointmentDbo?> GetAppointmentByReportAsync(
        string reportFileName, int userId)

    {
        return await QuerySingleAsync<AppointmentDbo>("sp_GetAppointmentByReport",

            new
            {
                ReportFileName = reportFileName,
                UserId = userId
            });
    }
        

}
