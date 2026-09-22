using LifeSure.Data;
using LifeSure.Entities;
using LifeSure.Mediator.Testimonials.Commands;
using LifeSure.Mediator.Testimonials.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.Mediator.Testimonials.Handlers;

public class SaveTestimonialCommandHandler(LifeSureDbContext context)
    : IRequestHandler<SaveTestimonialCommand, int?>
{
    public async Task<int?> Handle(
        SaveTestimonialCommand request,
        CancellationToken cancellationToken)
    {
        var input = request.Input;

        TestimonialInputValidator.Validate(input);

        Testimonial testimonial;

        if (request.Id.HasValue)
        {
            var existing = await context.Testimonials
                .Include(x => x.Translations)
                .SingleOrDefaultAsync(
                    x => x.Id == request.Id.Value,
                    cancellationToken);

            if (existing is null)
            {
                return null;
            }

            testimonial = existing;
        }
        else
        {
            testimonial = new Testimonial();
            context.Testimonials.Add(testimonial);
        }

        testimonial.FullName = input.FullName.Trim();
        testimonial.ImageUrl = input.ImageUrl;
        testimonial.Rating = input.Rating;
        testimonial.IsActive = input.IsActive;
        testimonial.DisplayOrder = input.DisplayOrder;

        foreach (var translationInput in input.Translations)
        {
            var translation = testimonial.Translations
                .FirstOrDefault(
                    x => x.LanguageCode == translationInput.LanguageCode);

            if (translation is null)
            {
                translation = new TestimonialTranslation
                {
                    LanguageCode = translationInput.LanguageCode
                };

                testimonial.Translations.Add(translation);
            }

            translation.Title = translationInput.Title.Trim();
            translation.Comment = translationInput.Comment.Trim();
        }

        var submittedLanguages = input.Translations
            .Select(x => x.LanguageCode)
            .ToHashSet();

        var removedTranslations = testimonial.Translations
            .Where(x => !submittedLanguages.Contains(x.LanguageCode))
            .ToList();

        context.TestimonialTranslations.RemoveRange(removedTranslations);

        await context.SaveChangesAsync(cancellationToken);

        return testimonial.Id;
    }
}

public class DeleteTestimonialCommandHandler(LifeSureDbContext context)
    : IRequestHandler<DeleteTestimonialCommand, bool>
{
    public async Task<bool> Handle(
        DeleteTestimonialCommand request,
        CancellationToken cancellationToken)
    {
        var testimonial = await context.Testimonials
            .SingleOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        if (testimonial is null)
        {
            return false;
        }

        context.Testimonials.Remove(testimonial);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}