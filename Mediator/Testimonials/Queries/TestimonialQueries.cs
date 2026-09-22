using LifeSure.Mediator.Testimonials.Results;
using MediatR;

namespace LifeSure.Mediator.Testimonials.Queries;

public record GetTestimonialsQuery(
    bool OnlyActive = false) : IRequest<List<TestimonialResult>>;

public record GetTestimonialByIdQuery(
    int Id) : IRequest<TestimonialResult?>;