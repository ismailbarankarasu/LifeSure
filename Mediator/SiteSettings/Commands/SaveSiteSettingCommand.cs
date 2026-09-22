using LifeSure.Mediator.SiteSettings.Models;
using MediatR;

namespace LifeSure.Mediator.SiteSettings.Commands;

public record SaveSiteSettingCommand(
    SiteSettingInput Input) : IRequest<int>;