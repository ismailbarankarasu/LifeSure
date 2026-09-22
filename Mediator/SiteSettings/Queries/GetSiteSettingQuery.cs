using LifeSure.Mediator.SiteSettings.Results;
using MediatR;

namespace LifeSure.Mediator.SiteSettings.Queries;

public record GetSiteSettingQuery() : IRequest<SiteSettingResult?>;