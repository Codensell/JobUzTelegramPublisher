using JobVacancyBot.Domain.Entities;

namespace JobVacancyBot.Application.Abstractions;

public interface IVacancyRepository
{
    Task<bool> ExistsByExternalIdAsync(
        string externalId,
        CancellationToken cancellationToken);

    Task AddAsync(
        Vacancy vacancy,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Vacancy>> GetUnpublishedAsync(
        int limit,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
