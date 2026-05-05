using JobVacancyBot.App.Formatting;
using JobVacancyBot.Application.Abstractions;
using JobVacancyBot.Infrastructure.VacancySources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>()
    .Build();

ServiceCollection services = new();

services.AddSingleton<IConfiguration>(configuration);

services.AddScoped<IVacancySource, FakeVacancySource>();
services.AddScoped<VacancyMessageFormatter>();

ServiceProvider serviceProvider = services.BuildServiceProvider();

string? channelId = configuration["Telegram:ChannelId"];
string? botToken = configuration["Telegram:BotToken"];

Console.WriteLine($"ChannelId: {channelId}");
Console.WriteLine(string.IsNullOrWhiteSpace(botToken)
    ? "BotToken: not configured"
    : "BotToken: configured");

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