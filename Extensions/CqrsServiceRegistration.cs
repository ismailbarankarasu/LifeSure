using LifeSure.CQRS.Abouts.Commands;
using LifeSure.CQRS.Abouts.Handlers;
using LifeSure.CQRS.Abouts.Queries;
using LifeSure.CQRS.Abouts.Results;
using LifeSure.CQRS.Abstractions;
using LifeSure.CQRS.Faqs.Commands;
using LifeSure.CQRS.Faqs.Handlers;
using LifeSure.CQRS.Faqs.Queries;
using LifeSure.CQRS.Faqs.Results;
using LifeSure.CQRS.Features.Commands;
using LifeSure.CQRS.Features.Handlers;
using LifeSure.CQRS.Features.Queries;
using LifeSure.CQRS.Features.Results;
using LifeSure.CQRS.Sliders.Commands;
using LifeSure.CQRS.Sliders.Handlers;
using LifeSure.CQRS.Sliders.Queries;
using LifeSure.CQRS.Sliders.Results;
using LifeSure.CQRS.Statistics.Commands;
using LifeSure.CQRS.Statistics.Handlers;
using LifeSure.CQRS.Statistics.Queries;
using LifeSure.CQRS.Statistics.Results;

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

        services.AddScoped<
            IQueryHandler<GetFeaturesQuery, List<FeatureResult>>,
            GetFeaturesQueryHandler>();

        services.AddScoped<
            IQueryHandler<GetFeatureByIdQuery, FeatureResult?>,
            GetFeatureByIdQueryHandler>();

        services.AddScoped<
            ICommandHandler<CreateFeatureCommand, int>,
            CreateFeatureCommandHandler>();

        services.AddScoped<
            ICommandHandler<UpdateFeatureCommand, bool>,
            UpdateFeatureCommandHandler>();

        services.AddScoped<
            ICommandHandler<DeleteFeatureCommand, bool>,
            DeleteFeatureCommandHandler>();

        services.AddScoped<
            IQueryHandler<GetAboutsQuery, List<AboutResult>>,
            GetAboutsQueryHandler>();

        services.AddScoped<
            IQueryHandler<GetAboutByIdQuery, AboutResult?>,
            GetAboutByIdQueryHandler>();

        services.AddScoped<
            ICommandHandler<SaveAboutCommand, int?>,
            SaveAboutCommandHandler>();

        services.AddScoped<
            ICommandHandler<DeleteAboutCommand, bool>,
            DeleteAboutCommandHandler>();

        services.AddScoped<
            IQueryHandler<GetStatisticsQuery, List<StatisticResult>>,
            GetStatisticsQueryHandler>();

        services.AddScoped<
            IQueryHandler<GetStatisticByIdQuery, StatisticResult?>,
            GetStatisticByIdQueryHandler>();

        services.AddScoped<
            ICommandHandler<SaveStatisticCommand, int?>,
            SaveStatisticCommandHandler>();

        services.AddScoped<
            ICommandHandler<DeleteStatisticCommand, bool>,
            DeleteStatisticCommandHandler>();

        services.AddScoped<
            IQueryHandler<GetFaqsQuery, List<FaqResult>>,
            GetFaqsQueryHandler>();

        services.AddScoped<
            IQueryHandler<GetFaqByIdQuery, FaqResult?>,
            GetFaqByIdQueryHandler>();

        services.AddScoped<
            ICommandHandler<SaveFaqCommand, int?>,
            SaveFaqCommandHandler>();

        services.AddScoped<
            ICommandHandler<DeleteFaqCommand, bool>,
            DeleteFaqCommandHandler>();
        return services;
    }
}