namespace LifeSure.CQRS.Sliders.Queries;

public record GetSlidersQuery(bool OnlyActive = false);