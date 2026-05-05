using JobVacancyBot.App.Formatting;
using JobVacancyBot.App.Telegram;
using JobVacancyBot.Application.Abstractions;
using JobVacancyBot.Application.UseCases;
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

services.AddSingleton(serviceProvider => new TelegramChannelPublisher(
    serviceProvider.GetRequiredService<ITelegramBotClient>(),
    channelId));

services.AddScoped<IVacancySource, FakeVacancySource>();
services.AddScoped<IVacancyPublisher, TelegramVacancyPublisher>();
services.AddScoped<VacancyMessageFormatter>();
services.AddScoped<PublishVacanciesUseCase>();

ServiceProvider serviceProvider = services.BuildServiceProvider();

PublishVacanciesUseCase useCase = serviceProvider.GetRequiredService<PublishVacanciesUseCase>();

await useCase.ExecuteAsync(CancellationToken.None);

Console.WriteLine("Vacancies were published to Telegram channel.");