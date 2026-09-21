namespace LifeSure.CQRS.Abouts.Queries;

public record GetAboutsQuery(bool OnlyActive = false);

public record GetAboutByIdQuery(int Id);