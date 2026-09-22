using LifeSure.Mediator.Testimonials.Models;
using MediatR;

namespace LifeSure.Mediator.Testimonials.Commands;

public record SaveTestimonialCommand(
    int? Id,
    TestimonialInput Input) : IRequest<int?>;

public record DeleteTestimonialCommand(int Id) : IRequest<bool>;