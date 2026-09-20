using LifeSure.Entities;
using LifeSure.Mediator.Services.Models;

namespace LifeSure.Mediator.Services.Results;

public record ServiceResult(
    int Id,
    string ImageUrl,
    string IconClass,
    bool IsActive,
    int DisplayOrder,
    IReadOnlyList<ServiceTranslationDto> Translations)
{
    public static ServiceResult FromEntity(Service service)
    {
        return new ServiceResult(
            service.Id,
            service.ImageUrl,
            service.IconClass,
            service.IsActive,
            service.DisplayOrder,
            service.Translations
                .OrderBy(x => x.LanguageCode)
                .Select(x => new ServiceTranslationDto
                {
                    LanguageCode = x.LanguageCode,
                    Title = x.Title,
                    ShortDescription = x.ShortDescription,
                    Description = x.Description
                })
                .ToList());
    }
}