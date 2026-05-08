using JobVacancyBot.Application.Abstractions;
using JobVacancyBot.Application.Options;
using JobVacancyBot.Domain.Entities;
using Microsoft.Extensions.Options;

namespace JobVacancyBot.Application.UseCases;

public sealed class CollectVacanciesUseCase
{
    private readonly IVacancySource _vacancySource;
    private readonly IVacancyRepository _vacancyRepository;
    private readonly PublishingOptions _options;

    public CollectVacanciesUseCase(
        IVacancySource vacancySource,
        IVacancyRepository vacancyRepository,
        IOptions<PublishingOptions> options)
    {
        _vacancySource = vacancySource;
        _vacancyRepository = vacancyRepository;
        _options = options.Value;
    }

    public async Task<int> ExecuteAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Vacancy> vacancies = await _vacancySource.GetVacanciesAsync(
            cancellationToken);

        int addedCount = 0;
        HashSet<string> addedExternalIds = [];

        foreach (Vacancy vacancy in vacancies)
        {
            if (addedCount >= _options.MaxVacanciesToCollectPerRun)
            {
                break;
            }

            if (!addedExternalIds.Add(vacancy.ExternalId))
            {
                continue;
            }

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
