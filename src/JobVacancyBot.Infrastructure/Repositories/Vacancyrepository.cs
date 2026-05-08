using JobVacancyBot.Application.Abstractions;
using JobVacancyBot.Domain.Entities;
using JobVacancyBot.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JobVacancyBot.Infrastructure.Repositories;

public sealed class VacancyRepository : IVacancyRepository
{
    private readonly AppDbContext _dbContext;

    public VacancyRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExistsByExternalIdAsync(
        string externalId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Vacancies.AnyAsync(
            vacancy => vacancy.ExternalId == externalId,
            cancellationToken);
    }

    public async Task AddAsync(
        Vacancy vacancy,
        CancellationToken cancellationToken)
    {
        await _dbContext.Vacancies.AddAsync(
            vacancy,
            cancellationToken);
    }

    public async Task<IReadOnlyList<Vacancy>> GetUnpublishedAsync(
        int limit,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Vacancies
            .Where(vacancy => vacancy.PublishedToTelegramAt == null)
            .OrderByDescending(vacancy => vacancy.PublishedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Vacancy>> GetRecentAsync(
        int limit,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Vacancies
            .OrderByDescending(vacancy => vacancy.PublishedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsPublicationAsync(
        Guid vacancyId,
        string channelKey,
        CancellationToken cancellationToken)
    {
        return await _dbContext.VacancyPublications.AnyAsync(
            publication =>
                publication.VacancyId == vacancyId &&
                publication.ChannelKey == channelKey,
            cancellationToken);
    }

    public async Task AddPublicationAsync(
        VacancyPublication publication,
        CancellationToken cancellationToken)
    {
        await _dbContext.VacancyPublications.AddAsync(
            publication,
            cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
