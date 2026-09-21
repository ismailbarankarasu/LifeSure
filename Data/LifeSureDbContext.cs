using LifeSure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.Data;

public class LifeSureDbContext : IdentityDbContext<IdentityUser>
{
    public LifeSureDbContext(DbContextOptions<LifeSureDbContext> options)
        : base(options)
    {
    }

    public DbSet<Slider> Sliders => Set<Slider>();
    public DbSet<SliderTranslation> SliderTranslations => Set<SliderTranslation>();

    public DbSet<Feature> Features => Set<Feature>();
    public DbSet<FeatureTranslation> FeatureTranslations => Set<FeatureTranslation>();

    public DbSet<About> Abouts => Set<About>();
    public DbSet<AboutTranslation> AboutTranslations => Set<AboutTranslation>();

    public DbSet<Statistic> Statistics => Set<Statistic>();
    public DbSet<StatisticTranslation> StatisticTranslations => Set<StatisticTranslation>();

    public DbSet<Faq> Faqs => Set<Faq>();
    public DbSet<FaqTranslation> FaqTranslations => Set<FaqTranslation>();

    public DbSet<Service> Services => Set<Service>();
    public DbSet<ServiceTranslation> ServiceTranslations => Set<ServiceTranslation>();

    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
    public DbSet<TeamMemberTranslation> TeamMemberTranslations => Set<TeamMemberTranslation>();

    public DbSet<Testimonial> Testimonials => Set<Testimonial>();
    public DbSet<TestimonialTranslation> TestimonialTranslations => Set<TestimonialTranslation>();

    public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();
    public DbSet<SiteSettingTranslation> SiteSettingTranslations => Set<SiteSettingTranslation>();

    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        LifeSureModelConfiguration.Configure(modelBuilder);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        UpdateTimestamps();

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();

        return base.SaveChangesAsync(
            acceptAllChangesOnSuccess,
            cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = null;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(x => x.CreatedAt).IsModified = false;
                entry.Entity.UpdatedAt = now;
            }
        }
    }
}