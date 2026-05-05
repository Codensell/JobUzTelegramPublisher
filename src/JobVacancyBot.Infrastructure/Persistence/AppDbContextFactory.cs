using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace JobVacancyBot.Infrastructure.Persistence;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<AppDbContext> optionsBuilder = new();

        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=job_vacancy_bot;Username=postgres;Password=postgres");

        return new AppDbContext(optionsBuilder.Options);
    }
}