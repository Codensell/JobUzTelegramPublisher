using System.Net.Http.Headers;
using JobVacancyBot.App.Formatting;
using JobVacancyBot.App.Telegram;
using JobVacancyBot.Application.Abstractions;
using JobVacancyBot.Application.UseCases;
using JobVacancyBot.Application.Options;
using JobVacancyBot.Infrastructure.Persistence;
using JobVacancyBot.Infrastructure.Repositories;
using JobVacancyBot.Infrastructure.HeadHunter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;

IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>()
    .Build();

string? botToken = configuration["Telegram:BotToken"];

if (string.IsNullOrWhiteSpace(botToken))
{
    Console.WriteLine("Telegram BotToken is not configured.");
    return;
}

ServiceCollection services = new();

services.AddSingleton<IConfiguration>(configuration);
services.Configure<PublishingOptions>(
    configuration.GetSection("Publishing"));

services.Configure<HeadHunterOptions>(
    configuration.GetSection("HeadHunter"));

services.Configure<VacancyCollectorOptions>(
    configuration.GetSection("VacancyCollector"));

services.Configure<TelegramChannelsOptions>(
    configuration.GetSection("TelegramChannels"));

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

services.AddSingleton<TelegramChannelPublisher>();

services.AddHttpClient<IVacancySource, HhVacancySource>((serviceProvider, httpClient) =>
{
    HeadHunterOptions options = serviceProvider
        .GetRequiredService<Microsoft.Extensions.Options.IOptions<HeadHunterOptions>>()
        .Value;

    httpClient.BaseAddress = new Uri(options.BaseUrl);
    httpClient.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(options.UserAgent);
    httpClient.DefaultRequestHeaders.Add("HH-User-Agent", options.UserAgent);

    if (!string.IsNullOrWhiteSpace(options.AccessToken))
    {
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", options.AccessToken);
    }
});

services.AddScoped<IVacancyPublisher, TelegramVacancyPublisher>();
services.AddScoped<VacancyMessageFormatter>();
services.AddScoped<CollectVacanciesUseCase>();
services.AddScoped<PublishVacanciesUseCase>();

using ServiceProvider serviceProvider = services.BuildServiceProvider();

VacancyCollectorOptions collectorOptions = serviceProvider
    .GetRequiredService<IOptions<VacancyCollectorOptions>>()
    .Value;

TimeSpan collectionInterval = TimeSpan.FromMinutes(
    collectorOptions.IntervalMinutes > 0
        ? collectorOptions.IntervalMinutes
        : 60);

using CancellationTokenSource shutdownTokenSource = new();

Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    shutdownTokenSource.Cancel();
};

Console.WriteLine($"Vacancy collector started. Interval: {collectionInterval.TotalMinutes:N0} minutes.");
Console.WriteLine("Press Ctrl+C to stop.");

while (!shutdownTokenSource.IsCancellationRequested)
{
    await RunCollectorCycleAsync(
        serviceProvider,
        shutdownTokenSource.Token);

    try
    {
        await Task.Delay(
            collectionInterval,
            shutdownTokenSource.Token);
    }
    catch (OperationCanceledException)
    {
        break;
    }
}

Console.WriteLine("Vacancy collector stopped.");

static async Task RunCollectorCycleAsync(
    IServiceProvider serviceProvider,
    CancellationToken cancellationToken)
{
    try
    {
        using IServiceScope scope = serviceProvider.CreateScope();

        CollectVacanciesUseCase collectUseCase =
            scope.ServiceProvider.GetRequiredService<CollectVacanciesUseCase>();

        PublishVacanciesUseCase publishUseCase =
            scope.ServiceProvider.GetRequiredService<PublishVacanciesUseCase>();

        Console.WriteLine($"Collector cycle started at {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss zzz}.");

        int addedCount = await collectUseCase.ExecuteAsync(cancellationToken);

        int publishedCount = await publishUseCase.ExecuteAsync(cancellationToken);

        Console.WriteLine($"Added vacancies: {addedCount}");
        Console.WriteLine($"Published vacancies: {publishedCount}");
    }
    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
    {
    }
    catch (Exception exception)
    {
        Console.WriteLine($"Collector cycle failed: {exception.Message}");
    }
}
