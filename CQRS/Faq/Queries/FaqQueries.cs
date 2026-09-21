namespace LifeSure.CQRS.Faqs.Queries;

public record GetFaqsQuery(bool OnlyActive = false);

public record GetFaqByIdQuery(int Id);