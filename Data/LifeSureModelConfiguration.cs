using LifeSure.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.Data;

public static class LifeSureModelConfiguration
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        ConfigureTranslations(modelBuilder);
        ConfigureFields(modelBuilder);
        ConfigureNotifications(modelBuilder);
    }

    private static void ConfigureTranslations(ModelBuilder modelBuilder)
    {
        ConfigureTranslation<Slider, SliderTranslation>(modelBuilder);
        ConfigureTranslation<Feature, FeatureTranslation>(modelBuilder);
        ConfigureTranslation<About, AboutTranslation>(modelBuilder);
        ConfigureTranslation<Statistic, StatisticTranslation>(modelBuilder);
        ConfigureTranslation<Faq, FaqTranslation>(modelBuilder);
        ConfigureTranslation<Service, ServiceTranslation>(modelBuilder);
        ConfigureTranslation<TeamMember, TeamMemberTranslation>(modelBuilder);
        ConfigureTranslation<Testimonial, TestimonialTranslation>(modelBuilder);
        ConfigureTranslation<SiteSetting, SiteSettingTranslation>(modelBuilder);
    }

    private static void ConfigureTranslation<TParent, TTranslation>(
        ModelBuilder modelBuilder)
        where TParent : BaseEntity
        where TTranslation : BaseEntity
    {
        var parentName = typeof(TParent).Name;
        var foreignKeyName = $"{parentName}Id";

        var entity = modelBuilder.Entity<TTranslation>();

        entity.Property<string>("LanguageCode")
            .HasMaxLength(10)
            .IsRequired();

        entity.HasIndex(foreignKeyName, "LanguageCode")
            .IsUnique();

        entity.HasOne<TParent>(parentName)
            .WithMany("Translations")
            .HasForeignKey(foreignKeyName)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureFields(ModelBuilder modelBuilder)
    {
        // Slider
        modelBuilder.Entity<Slider>(entity =>
        {
            entity.Property(x => x.ImageUrl).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.VideoId).HasMaxLength(11);
            entity.Property(x => x.ButtonUrl).HasMaxLength(1000).IsRequired();
        });

        modelBuilder.Entity<SliderTranslation>(entity =>
        {
            entity.Property(x => x.Subtitle).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            entity.Property(x => x.ButtonText).HasMaxLength(100).IsRequired();
        });

        // Özellikler
        modelBuilder.Entity<Feature>()
            .Property(x => x.IconClass).HasMaxLength(100).IsRequired();

        modelBuilder.Entity<FeatureTranslation>(entity =>
        {
            entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(2000).IsRequired();
        });

        // Hakkımızda
        modelBuilder.Entity<About>()
            .Property(x => x.ImageUrl).HasMaxLength(1000).IsRequired();

        modelBuilder.Entity<AboutTranslation>(entity =>
        {
            entity.Property(x => x.Subtitle).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(6000).IsRequired();
        });

        // İstatistikler
        modelBuilder.Entity<Statistic>(entity =>
        {
            entity.Property(x => x.Suffix).HasMaxLength(20);

            entity.ToTable("Statistics", table =>
            {
                table.HasCheckConstraint(
                    "CK_Statistics_Value",
                    "[Value] >= 0");

                table.HasCheckConstraint(
                    "CK_Statistics_Source",
                    "[Source] IN (0, 1, 2)");
            });
        });

        modelBuilder.Entity<StatisticTranslation>()
            .Property(x => x.Title).HasMaxLength(200).IsRequired();

        // SSS
        modelBuilder.Entity<FaqTranslation>(entity =>
        {
            entity.Property(x => x.Question).HasMaxLength(500).IsRequired();
            entity.Property(x => x.Answer).HasMaxLength(6000).IsRequired();
        });

        // Hizmetler
        modelBuilder.Entity<Service>(entity =>
        {
            entity.Property(x => x.ImageUrl).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.IconClass).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<ServiceTranslation>(entity =>
        {
            entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
            entity.Property(x => x.ShortDescription).HasMaxLength(500).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(6000).IsRequired();
        });

        // Ekip
        modelBuilder.Entity<TeamMember>(entity =>
        {
            entity.Property(x => x.FullName).HasMaxLength(150).IsRequired();
            entity.Property(x => x.ImageUrl).HasMaxLength(1000).IsRequired();

            entity.Property(x => x.FacebookUrl).HasMaxLength(1000);
            entity.Property(x => x.InstagramUrl).HasMaxLength(1000);
            entity.Property(x => x.LinkedInUrl).HasMaxLength(1000);
            entity.Property(x => x.XUrl).HasMaxLength(1000);
        });

        modelBuilder.Entity<TeamMemberTranslation>()
            .Property(x => x.JobTitle).HasMaxLength(150).IsRequired();

        // Referanslar
        modelBuilder.Entity<Testimonial>(entity =>
        {
            entity.Property(x => x.FullName).HasMaxLength(150).IsRequired();
            entity.Property(x => x.ImageUrl).HasMaxLength(1000).IsRequired();

            entity.ToTable("Testimonials", table =>
                table.HasCheckConstraint(
                    "CK_Testimonials_Rating",
                    "[Rating] BETWEEN 1 AND 5"));
        });

        modelBuilder.Entity<TestimonialTranslation>(entity =>
        {
            entity.Property(x => x.Title).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Comment).HasMaxLength(3000).IsRequired();
        });

        // Site ayarları
        modelBuilder.Entity<SiteSetting>(entity =>
        {
            entity.Property(x => x.SiteName).HasMaxLength(150).IsRequired();
            entity.Property(x => x.LogoUrl).HasMaxLength(1000);
            entity.Property(x => x.Email).HasMaxLength(254).IsRequired();
            entity.Property(x => x.PhoneNumber).HasMaxLength(30).IsRequired();
            entity.Property(x => x.Address).HasMaxLength(500).IsRequired();

            entity.Property(x => x.MapUrl).HasMaxLength(2000);
            entity.Property(x => x.FacebookUrl).HasMaxLength(1000);
            entity.Property(x => x.InstagramUrl).HasMaxLength(1000);
            entity.Property(x => x.LinkedInUrl).HasMaxLength(1000);
            entity.Property(x => x.XUrl).HasMaxLength(1000);
        });

        modelBuilder.Entity<SiteSettingTranslation>(entity =>
        {
            entity.Property(x => x.FooterDescription).HasMaxLength(2000).IsRequired();
            entity.Property(x => x.MetaDescription).HasMaxLength(500).IsRequired();
        });

        // İletişim mesajları
        modelBuilder.Entity<ContactMessage>(entity =>
        {
            entity.Property(x => x.FullName).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(254).IsRequired();
            entity.Property(x => x.PhoneNumber).HasMaxLength(30);
            entity.Property(x => x.Subject).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Message).HasMaxLength(6000).IsRequired();

            entity.HasIndex(x => new { x.Status, x.CreatedAt });

            entity.ToTable("ContactMessages", table =>
                table.HasCheckConstraint(
                    "CK_ContactMessages_Status",
                    "[Status] IN (0, 1, 2)"));
        });
    }

    private static void ConfigureNotifications(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(1000).IsRequired();

            entity.HasIndex(x => new { x.IsRead, x.CreatedAt });

            entity.HasOne(x => x.ContactMessage)
                .WithMany()
                .HasForeignKey(x => x.ContactMessageId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}