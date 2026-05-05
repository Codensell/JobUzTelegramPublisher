using JobVacancyBot.App.Formatting;
using JobVacancyBot.Application.Abstractions;
using JobVacancyBot.Infrastructure.VacancySources;
using Microsoft.Extensions.DependencyInjection;

ServiceCollection services = new();

services.AddScoped<IVacancySource, FakeVacancySource>();
services.AddScoped<VacancyMessageFormatter>();

ServiceProvider serviceProvider = services.BuildServiceProvider();

IVacancySource vacancySource = serviceProvider.GetRequiredService<IVacancySource>();
VacancyMessageFormatter formatter = serviceProvider.GetRequiredService<VacancyMessageFormatter>();

IReadOnlyList<JobVacancyBot.Domain.Entities.Vacancy> vacancies =
    await vacancySource.GetVacanciesAsync(CancellationToken.None);

foreach (JobVacancyBot.Domain.Entities.Vacancy vacancy in vacancies)
{
    string message = formatter.Format(vacancy);

    Console.WriteLine(message);
    Console.WriteLine("----------------------------------------");
}