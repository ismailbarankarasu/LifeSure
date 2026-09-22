using LifeSure.Entities;
using LifeSure.Mediator.Testimonials.Models;

namespace LifeSure.Mediator.Testimonials.Results;

public record TestimonialResult(
    int Id,
    string FullName,
    string ImageUrl,
    int Rating,
    bool IsActive,
    int DisplayOrder,
    IReadOnlyList<TestimonialTranslationDto> Translations)
{
    public static TestimonialResult FromEntity(Testimonial testimonial)
    {
        return new TestimonialResult(
            testimonial.Id,
            testimonial.FullName,
            testimonial.ImageUrl,
            testimonial.Rating,
            testimonial.IsActive,
            testimonial.DisplayOrder,
            testimonial.Translations
                .OrderBy(x => x.LanguageCode)
                .Select(x => new TestimonialTranslationDto
                {
                    LanguageCode = x.LanguageCode,
                    Title = x.Title,
                    Comment = x.Comment
                })
                .ToList());
    }
}