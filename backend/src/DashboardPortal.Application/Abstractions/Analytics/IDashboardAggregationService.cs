using DashboardPortal.Domain.Configurations;

namespace DashboardPortal.Application.Abstractions.Analytics;

public interface IDashboardAggregationService
{
    Task<IReadOnlyList<WidgetResult>> RunAsync(
        DashboardConfiguration configuration,
        CancellationToken cancellationToken = default);
}
