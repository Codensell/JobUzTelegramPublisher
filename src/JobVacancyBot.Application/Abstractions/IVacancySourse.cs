using JobVacancyBot.Domain.Entities;

namespace JobVacancyBot.Application.Abstractions;

public interface IVacancySource
{
    Task<IReadOnlyList<Vacancy>> GetVacanciesAsync(CancellationToken cancellationToken);
}