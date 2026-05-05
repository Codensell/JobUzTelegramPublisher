using JobVacancyBot.App.Formatting;
using JobVacancyBot.App.Telegram;
using JobVacancyBot.Application.Abstractions;
using JobVacancyBot.Infrastructure.VacancySources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>()
    .Build();

string? botToken = configuration["Telegram:BotToken"];
string? channelId = configuration["Telegram:ChannelId"];

if (string.IsNullOrWhiteSpace(botToken))
{
    Console.WriteLine("Telegram BotToken is not configured.");
    return;
}

if (string.IsNullOrWhiteSpace(channelId))
{
    Console.WriteLine("Telegram ChannelId is not configured.");
    return;
}

ServiceCollection services = new();

services.AddSingleton<IConfiguration>(configuration);

services.AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(botToken));
services.AddSingleton(_ => new TelegramChannelPublisher(
    _.GetRequiredService<ITelegramBotClient>(),
    channelId));

services.AddScoped<IVacancySource, FakeVacancySource>();
services.AddScoped<VacancyMessageFormatter>();

ServiceProvider serviceProvider = services.BuildServiceProvider();

IVacancySource vacancySource = serviceProvider.GetRequiredService<IVacancySource>();
VacancyMessageFormatter formatter = serviceProvider.GetRequiredService<VacancyMessageFormatter>();
TelegramChannelPublisher publisher = serviceProvider.GetRequiredService<TelegramChannelPublisher>();

IReadOnlyList<JobVacancyBot.Domain.Entities.Vacancy> vacancies =
    await vacancySource.GetVacanciesAsync(CancellationToken.None);

JobVacancyBot.Domain.Entities.Vacancy firstVacancy = vacancies[0];

string message = formatter.Format(firstVacancy);

await publisher.PublishAsync(message, CancellationToken.None);

Console.WriteLine("Test vacancy was published to Telegram channel.");