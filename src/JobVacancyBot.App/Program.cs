using JobVacancyBot.App.Formatting;
using JobVacancyBot.App.Telegram;
using JobVacancyBot.Application.Abstractions;
using JobVacancyBot.Application.UseCases;
using JobVacancyBot.Infrastructure.VacancySources;
using JobVacancyBot.Application.Options;
using JobVacancyBot.Infrastructure.Persistence;
using JobVacancyBot.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
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
services.Configure<PublishingOptions>(
    configuration.GetSection("Publishing"));

string? connectionString = configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("DefaultConnection is not configured.");
    return;
}

services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});
services.AddScoped<IVacancyRepository, VacancyRepository>();

services.AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(botToken));

services.AddSingleton(serviceProvider => new TelegramChannelPublisher(
    serviceProvider.GetRequiredService<ITelegramBotClient>(),
    channelId));

services.AddScoped<IVacancySource, FakeVacancySource>();
services.AddScoped<IVacancyPublisher, TelegramVacancyPublisher>();
services.AddScoped<VacancyMessageFormatter>();
services.AddScoped<CollectVacanciesUseCase>();
services.AddScoped<PublishVacanciesUseCase>();

ServiceProvider serviceProvider = services.BuildServiceProvider();

CollectVacanciesUseCase collectUseCase =
    serviceProvider.GetRequiredService<CollectVacanciesUseCase>();

PublishVacanciesUseCase publishUseCase =
    serviceProvider.GetRequiredService<PublishVacanciesUseCase>();

int addedCount = await collectUseCase.ExecuteAsync(CancellationToken.None);

int publishedCount = await publishUseCase.ExecuteAsync(CancellationToken.None);

Console.WriteLine($"Added vacancies: {addedCount}");
Console.WriteLine($"Published vacancies: {publishedCount}");