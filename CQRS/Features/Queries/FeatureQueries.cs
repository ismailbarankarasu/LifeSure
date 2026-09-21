namespace LifeSure.CQRS.Features.Queries;

public record GetFeaturesQuery(bool OnlyActive = false);

public record GetFeatureByIdQuery(int Id);