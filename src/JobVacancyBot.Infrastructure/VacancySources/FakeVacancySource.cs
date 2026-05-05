using JobVacancyBot.Application.Abstractions;
using JobVacancyBot.Domain.Entities;

namespace JobVacancyBot.Infrastructure.VacancySources;

public sealed class FakeVacancySource : IVacancySource
{
    public Task<IReadOnlyList<Vacancy>> GetVacanciesAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Vacancy> vacancies =
        [
            new Vacancy(
                externalId: "fake-001",
                title: ".NET Backend Developer",
                companyName: "Example Tech",
                areaName: "Ташкент",
                salaryFrom: 8000000,
                salaryTo: 12000000,
                currency: "UZS",
                experienceName: "1–3 года",
                employmentName: "Полная занятость",
                scheduleName: "Полный день",
                requirementSnippet: "C#, ASP.NET Core, PostgreSQL, REST API.",
                responsibilitySnippet: "Разработка backend-сервисов и поддержка существующего API.",
                url: "https://hh.uz/vacancy/fake-001",
                publishedAt: DateTime.UtcNow),

            new Vacancy(
                externalId: "fake-002",
                title: "QA Engineer",
                companyName: "Quality Lab",
                areaName: "Узбекистан",
                salaryFrom: null,
                salaryTo: null,
                currency: null,
                experienceName: "Без опыта",
                employmentName: "Полная занятость",
                scheduleName: "Удалённая работа",
                requirementSnippet: "Понимание тест-кейсов, баг-репортов и клиент-серверной архитектуры.",
                responsibilitySnippet: "Ручное тестирование веб-приложений и оформление дефектов.",
                url: "https://hh.uz/vacancy/fake-002",
                publishedAt: DateTime.UtcNow)
        ];

        return Task.FromResult(vacancies);
    }
}