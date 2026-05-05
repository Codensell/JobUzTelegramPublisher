using JobVacancyBot.Application.Abstractions;
using JobVacancyBot.Application.Options;
using JobVacancyBot.Domain.Entities;
using Microsoft.Extensions.Options;

namespace JobVacancyBot.Application.UseCases;

public sealed class PublishVacanciesUseCase
{
    private readonly IVacancyRepository _vacancyRepository;
    private readonly IVacancyPublisher _vacancyPublisher;
    private readonly PublishingOptions _options;

    public PublishVacanciesUseCase(
        IVacancyRepository vacancyRepository,
        IVacancyPublisher vacancyPublisher,
        IOptions<PublishingOptions> options)
    {
        _vacancyRepository = vacancyRepository;
        _vacancyPublisher = vacancyPublisher;
        _options = options.Value;
    }

    public async Task<int> ExecuteAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Vacancy> vacancies = await _vacancyRepository.GetUnpublishedAsync(
            _options.MaxVacanciesPerRun,
            cancellationToken);

        int publishedCount = 0;

        foreach (Vacancy vacancy in vacancies)
        {
            await _vacancyPublisher.PublishAsync(
                vacancy,
                cancellationToken);

            vacancy.MarkAsPublishedToTelegram(DateTime.UtcNow);

            publishedCount++;
        }

        await _vacancyRepository.SaveChangesAsync(cancellationToken);

        return publishedCount;
    }
}