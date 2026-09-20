using LifeSure.CQRS.Abstractions;
using LifeSure.CQRS.Sliders.Commands;
using LifeSure.CQRS.Sliders.Handlers;
using LifeSure.CQRS.Sliders.Queries;
using LifeSure.CQRS.Sliders.Results;

namespace LifeSure.Extensions;

public static class CqrsServiceRegistration
{
    public static IServiceCollection AddCqrsHandlers(
        this IServiceCollection services)
    {
        services.AddScoped<
            IQueryHandler<GetSlidersQuery, List<SliderResult>>,
            GetSlidersQueryHandler>();

        services.AddScoped<
            IQueryHandler<GetSliderByIdQuery, SliderResult?>,
            GetSliderByIdQueryHandler>();

        services.AddScoped<
            ICommandHandler<CreateSliderCommand, int>,
            CreateSliderCommandHandler>();

        services.AddScoped<
            ICommandHandler<UpdateSliderCommand, bool>,
            UpdateSliderCommandHandler>();

        services.AddScoped<
            ICommandHandler<DeleteSliderCommand, bool>,
            DeleteSliderCommandHandler>();

        return services;
    }
}