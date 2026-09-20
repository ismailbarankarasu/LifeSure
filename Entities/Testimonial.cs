namespace LifeSure.Entities;

public class Testimonial : DisplayEntity
{
    public string FullName { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public int Rating { get; set; } = 5;

    public ICollection<TestimonialTranslation> Translations { get; set; } = new List<TestimonialTranslation>();
}

public class TestimonialTranslation : BaseEntity
{
    public int TestimonialId { get; set; }

    public Testimonial Testimonial { get; set; } = null!;

    public string LanguageCode { get; set; } = "tr";

    public string Title { get; set; } = string.Empty;

    public string Comment { get; set; } = string.Empty;
}