using LifeSure.Mediator.Services.Handlers;

namespace LifeSure.Extensions;

public static class MediatorServiceRegistration
{
    public static IServiceCollection AddMediatorHandlers(this IServiceCollection services)
    {
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblyContaining<GetServicesQueryHandler>());
        return services;
    }
}