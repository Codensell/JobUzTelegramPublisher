using JobVacancyBot.Domain.Entities;

namespace JobVacancyBot.Application.Abstractions;

public interface IVacancyPublisher
{
    Task PublishAsync(
        Vacancy vacancy,
        CancellationToken cancellationToken);
}