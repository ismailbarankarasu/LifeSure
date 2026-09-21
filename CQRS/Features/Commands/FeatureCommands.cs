using LifeSure.CQRS.Features.Models;

namespace LifeSure.CQRS.Features.Commands;

public record CreateFeatureCommand(FeatureInput Input);

public record UpdateFeatureCommand(int Id, FeatureInput Input);

public record DeleteFeatureCommand(int Id);