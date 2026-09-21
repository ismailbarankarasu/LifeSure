using LifeSure.CQRS.Abstractions;
using LifeSure.CQRS.Statistics.Queries;
using LifeSure.CQRS.Statistics.Results;
using LifeSure.Data;
using LifeSure.Enums;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.CQRS.Statistics.Handlers;

public class GetStatisticsQueryHandler(LifeSureDbContext context)
    : IQueryHandler<GetStatisticsQuery, List<StatisticResult>>
{
    public async Task<List<StatisticResult>> HandleAsync(
        GetStatisticsQuery query,
        CancellationToken cancellationToken = default)
    {
        var statisticsQuery = context.Statistics
            .AsNoTracking()
            .AsQueryable();

        if (query.OnlyActive)
        {
            statisticsQuery = statisticsQuery.Where(x => x.IsActive);
        }

        var statistics = await statisticsQuery
            .Include(x => x.Translations)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

        var teamCount = 0;
        var serviceCount = 0;

        if (statistics.Any(
                x => x.Source == StatisticSource.ActiveTeamMemberCount))
        {
            teamCount = await context.TeamMembers
                .CountAsync(x => x.IsActive, cancellationToken);
        }

        if (statistics.Any(
                x => x.Source == StatisticSource.ActiveServiceCount))
        {
            serviceCount = await context.Services
                .CountAsync(x => x.IsActive, cancellationToken);
        }

        return statistics.Select(statistic =>
        {
            var displayValue = statistic.Source switch
            {
                StatisticSource.ActiveTeamMemberCount => teamCount,
                StatisticSource.ActiveServiceCount => serviceCount,
                _ => statistic.Value
            };

            return StatisticResult.FromEntity(statistic, displayValue);
        }).ToList();
    }
}

public class GetStatisticByIdQueryHandler(LifeSureDbContext context)
    : IQueryHandler<GetStatisticByIdQuery, StatisticResult?>
{
    public async Task<StatisticResult?> HandleAsync(
        GetStatisticByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var statistic = await context.Statistics
            .AsNoTracking()
            .Include(x => x.Translations)
            .SingleOrDefaultAsync(
                x => x.Id == query.Id,
                cancellationToken);

        if (statistic is null)
        {
            return null;
        }

        var displayValue = statistic.Value;

        if (statistic.Source == StatisticSource.ActiveTeamMemberCount)
        {
            displayValue = await context.TeamMembers
                .CountAsync(x => x.IsActive, cancellationToken);
        }
        else if (statistic.Source == StatisticSource.ActiveServiceCount)
        {
            displayValue = await context.Services
                .CountAsync(x => x.IsActive, cancellationToken);
        }

        return StatisticResult.FromEntity(statistic, displayValue);
    }
}