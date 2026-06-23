# HomeSampling .NET 8 Backend

Modular Monolith backend for the HomeSampling medical home-sampling platform,
built with .NET 8 Web API, ADO.NET, and SQL Server stored procedures.

## Architecture

Modular Monolith — one deployable app, five independent feature modules,
each following a 3-layer pattern: Controller -> Service -> DBContext.

```
HomeSampling.API/              <- entry point (Program.cs, Middleware)
Shared.Infrastructure/         <- ApiResponse, BaseDBContext, Email/File/Guid services
Module.Auth(.Core/.Infrastructure)      <- OTP register/login/reset
Module.Patient(.Core/.Infrastructure)   <- tests, bookings, reports
Module.Admin(.Core/.Infrastructure)     <- dashboard, CRUD, rider assignment
Module.Rider(.Core/.Infrastructure)     <- task lifecycle, notifications
Module.Contact(.Core)                   <- public contact form (no DB)
```

## Setup steps

1. **Restore & build**
   ```
   dotnet restore
   dotnet build
   ```

2. **Create the SQL Server database**
   Create a database named `HomeSamplingDB` (or update the connection string
   in `appsettings.json`). You still need to write the stored procedures
   referenced throughout the DBContext classes, for example:
   - `sp_GetUserByEmail`, `sp_CreateUser`, `sp_CreateOtp`, `sp_GetLatestOtp`, `sp_MarkOtpUsed`, `sp_UpdatePassword`, `sp_IncrementLoginAttempts`, `sp_ResetLoginAttempts`
   - `sp_GetActiveTests`, `sp_CreateBooking`, `sp_GetAppointmentByDate`, `sp_GetPatientBookings`, `sp_GetAppointmentById`, `sp_CancelBooking`, `sp_GetAppointmentByReport`
   - `sp_GetDashboardStats`, `sp_GetAppointmentsPaged`, `sp_UpdateAppointmentStatus`, `sp_BulkUpdateStatus`, `sp_AssignRider`, `sp_AutoAssignRider`, `sp_SaveReportPath`, `sp_GetAllPatients`, `sp_GetAllTests`, `sp_CreateTest`, `sp_UpdateTest`, `sp_DeleteTest`, `sp_GetAllRiders`, `sp_GetRiderById`, `sp_CreateRider`, `sp_DeleteRider`
   - `sp_GetRiderByEmail`, `sp_GetActiveTasksByRider`, `sp_GetTaskHistoryByRider`, `sp_GetTaskByIdAndRider`, `sp_InsertTaskLog`, `sp_UpdateRiderLocation`, `sp_GetRiderNotifications`, `sp_MarkNotificationRead`, `sp_MarkAllNotificationsRead`

   Recommended tables: `Users`, `Otps`, `Tests`, `Appointments`, `Riders`, `TaskLogs`, `Notifications`.

3. **Update `appsettings.json`**
   - `ConnectionStrings:DefaultConnection` — your SQL Server connection string
   - `Jwt:Key` — replace with your own 32+ character secret
   - `Email:*` — your SMTP credentials (Gmail App Password recommended)
   - `AllowedOrigins` — your React dev server URL(s)

4. **Run**
   ```
   cd HomeSampling.API
   dotnet run
   ```
   Swagger UI opens automatically at `/swagger`.

## Notes

- All data access uses raw ADO.NET (`Microsoft.Data.SqlClient`) via stored
  procedures — see `Shared.Infrastructure/DBContext/BaseDBContext.cs` for the
  reusable query/execute helpers every module's DBContext extends.
- JWT auth supports three roles: `Patient`, `Admin`, `Rider`. Each module's
  login endpoint issues a token with the correct role claim.
- File uploads (reports, sample photos) are saved locally under
  `HomeSampling.API/uploads/` via `IFileService`.
