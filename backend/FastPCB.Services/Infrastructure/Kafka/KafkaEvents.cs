using FastPCB.Models;

namespace FastPCB.Services.Infrastructure.Kafka
{
    public sealed record ProjectCreated(
        int ProjectId,
        int UserId,
        string Title,
        DateTime CreatedAt);

    public sealed record ProjectReported(
        int ReportId,
        int ProjectId,
        int ReporterUserId,
        string Reason,
        DateTime CreatedAt);

    public sealed record ReportStatusChanged(
        int ReportId,
        TicketStatus OldStatus,
        TicketStatus NewStatus,
        DateTime ChangedAt);

    public sealed record UserRoleChanged(
        int UserId,
        UserRole OldRole,
        UserRole NewRole,
        DateTime ChangedAt);
}
