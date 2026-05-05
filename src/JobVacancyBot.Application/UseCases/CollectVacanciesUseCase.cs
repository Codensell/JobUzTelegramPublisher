using JobVacancyBot.Application.Abstractions;
using JobVacancyBot.Domain.Entities;

namespace JobVacancyBot.Application.UseCases;

public sealed class CollectVacanciesUseCase
{
    private readonly IVacancySource _vacancySource;
    private readonly IVacancyRepository _vacancyRepository;

    public CollectVacanciesUseCase(
        IVacancySource vacancySource,
        IVacancyRepository vacancyRepository)
    {
        _vacancySource = vacancySource;
        _vacancyRepository = vacancyRepository;
    }

    public async Task<int> ExecuteAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Vacancy> vacancies = await _vacancySource.GetVacanciesAsync(
            cancellationToken);

        int addedCount = 0;

        foreach (Vacancy vacancy in vacancies)
        {
            bool exists = await _vacancyRepository.ExistsByExternalIdAsync(
                vacancy.ExternalId,
                cancellationToken);

            if (exists)
            {
                continue;
            }

            await _vacancyRepository.AddAsync(
                vacancy,
                cancellationToken);

            addedCount++;
        }

        await _vacancyRepository.SaveChangesAsync(cancellationToken);

        return addedCount;
    }
}