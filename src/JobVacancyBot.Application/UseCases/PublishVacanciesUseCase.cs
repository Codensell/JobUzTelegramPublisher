using JobVacancyBot.Application.Abstractions;
using JobVacancyBot.Domain.Entities;

namespace JobVacancyBot.Application.UseCases;

public sealed class PublishVacanciesUseCase
{
    private readonly IVacancySource _vacancySource;
    private readonly IVacancyPublisher _vacancyPublisher;

    public PublishVacanciesUseCase(
        IVacancySource vacancySource,
        IVacancyPublisher vacancyPublisher)
    {
        _vacancySource = vacancySource;
        _vacancyPublisher = vacancyPublisher;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Vacancy> vacancies = await _vacancySource.GetVacanciesAsync(
            cancellationToken);

        foreach (Vacancy vacancy in vacancies)
        {
            await _vacancyPublisher.PublishAsync(
                vacancy,
                cancellationToken);
        }
    }
}