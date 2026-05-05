using JobVacancyBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobVacancyBot.Infrastructure.Persistence.Configurations;

public sealed class VacancyConfiguration : IEntityTypeConfiguration<Vacancy>
{
    public void Configure(EntityTypeBuilder<Vacancy> builder)
    {
        builder.ToTable("vacancies");

        builder.HasKey(vacancy => vacancy.Id);

        builder.Property(vacancy => vacancy.ExternalId)
            .IsRequired()
            .HasMaxLength(64);

        builder.HasIndex(vacancy => vacancy.ExternalId)
            .IsUnique();

        builder.Property(vacancy => vacancy.Title)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(vacancy => vacancy.CompanyName)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(vacancy => vacancy.AreaName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(vacancy => vacancy.Currency)
            .HasMaxLength(16);

        builder.Property(vacancy => vacancy.ExperienceName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(vacancy => vacancy.EmploymentName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(vacancy => vacancy.ScheduleName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(vacancy => vacancy.RequirementSnippet)
            .HasMaxLength(1000);

        builder.Property(vacancy => vacancy.ResponsibilitySnippet)
            .HasMaxLength(1000);

        builder.Property(vacancy => vacancy.Url)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(vacancy => vacancy.PublishedAt)
            .IsRequired();

        builder.Property(vacancy => vacancy.CreatedAt)
            .IsRequired();

        builder.Property(vacancy => vacancy.PublishedToTelegramAt);
    }
}