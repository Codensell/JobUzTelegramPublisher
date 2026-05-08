using JobVacancyBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobVacancyBot.Infrastructure.Persistence.Configurations;

public sealed class VacancyPublicationConfiguration : IEntityTypeConfiguration<VacancyPublication>
{
    public void Configure(EntityTypeBuilder<VacancyPublication> builder)
    {
        builder.ToTable("vacancy_publications");

        builder.HasKey(publication => publication.Id);

        builder.Property(publication => publication.ChannelKey)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(publication => publication.ChannelId)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(publication => publication.PublishedAt)
            .IsRequired();

        builder.HasIndex(publication => new
            {
                publication.VacancyId,
                publication.ChannelKey
            })
            .IsUnique();
    }
}
