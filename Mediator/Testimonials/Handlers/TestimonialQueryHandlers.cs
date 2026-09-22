using LifeSure.Data;
using LifeSure.Mediator.Testimonials.Queries;
using LifeSure.Mediator.Testimonials.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.Mediator.Testimonials.Handlers;

public class GetTestimonialsQueryHandler(LifeSureDbContext context)
    : IRequestHandler<GetTestimonialsQuery, List<TestimonialResult>>
{
    public async Task<List<TestimonialResult>> Handle(
        GetTestimonialsQuery request,
        CancellationToken cancellationToken)
    {
        var query = context.Testimonials
            .AsNoTracking()
            .AsQueryable();

        if (request.OnlyActive)
        {
            query = query.Where(x => x.IsActive);
        }

        var testimonials = await query
            .Include(x => x.Translations)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

        return testimonials
            .Select(TestimonialResult.FromEntity)
            .ToList();
    }
}

public class GetTestimonialByIdQueryHandler(LifeSureDbContext context)
    : IRequestHandler<GetTestimonialByIdQuery, TestimonialResult?>
{
    public async Task<TestimonialResult?> Handle(
        GetTestimonialByIdQuery request,
        CancellationToken cancellationToken)
    {
        var testimonial = await context.Testimonials
            .AsNoTracking()
            .Include(x => x.Translations)
            .SingleOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        return testimonial is null
            ? null
            : TestimonialResult.FromEntity(testimonial);
    }
}