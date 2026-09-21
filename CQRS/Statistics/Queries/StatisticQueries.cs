namespace LifeSure.CQRS.Statistics.Queries;

public record GetStatisticsQuery(bool OnlyActive = false);

public record GetStatisticByIdQuery(int Id);