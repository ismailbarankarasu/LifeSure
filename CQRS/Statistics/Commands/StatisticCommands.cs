using LifeSure.CQRS.Statistics.Models;

namespace LifeSure.CQRS.Statistics.Commands;

public record SaveStatisticCommand(int? Id, StatisticInput Input);

public record DeleteStatisticCommand(int Id);