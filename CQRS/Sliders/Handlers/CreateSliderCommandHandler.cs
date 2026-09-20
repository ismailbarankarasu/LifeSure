using LifeSure.CQRS.Abstractions;
using LifeSure.CQRS.Sliders.Commands;
using LifeSure.CQRS.Sliders.Models;
using LifeSure.Data;
using LifeSure.Entities;

namespace LifeSure.CQRS.Sliders.Handlers;

public class CreateSliderCommandHandler(LifeSureDbContext context)
    : ICommandHandler<CreateSliderCommand, int>
{
    public async Task<int> HandleAsync(
        CreateSliderCommand command,
        CancellationToken cancellationToken = default)
    {
        var input = command.Input;

        SliderInputValidator.Validate(input);

        var slider = new Slider
        {
            ImageUrl = input.ImageUrl,
            VideoId = string.IsNullOrEmpty(input.VideoId)
                ? null
                : input.VideoId,
            ButtonUrl = input.ButtonUrl,
            IsActive = input.IsActive,
            DisplayOrder = input.DisplayOrder,

            Translations = input.Translations
                .Select(x => new SliderTranslation
                {
                    LanguageCode = x.LanguageCode,
                    Subtitle = x.Subtitle,
                    Title = x.Title,
                    Description = x.Description,
                    ButtonText = x.ButtonText
                })
                .ToList()
        };

        context.Sliders.Add(slider);

        await context.SaveChangesAsync(cancellationToken);

        return slider.Id;
    }
}