using JobVacancyBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobVacancyBot.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public DbSet<Vacancy> Vacancies => Set<Vacancy>();
    public DbSet<VacancyPublication> VacancyPublications => Set<VacancyPublication>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
