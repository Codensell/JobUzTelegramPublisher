using JobVacancyBot.App.Formatting;
using JobVacancyBot.Application.Abstractions;
using JobVacancyBot.Domain.Entities;

namespace JobVacancyBot.App.Telegram;

public sealed class TelegramVacancyPublisher : IVacancyPublisher
{
    private readonly TelegramChannelPublisher _channelPublisher;
    private readonly VacancyMessageFormatter _formatter;

    public TelegramVacancyPublisher(
        TelegramChannelPublisher channelPublisher,
        VacancyMessageFormatter formatter)
    {
        _channelPublisher = channelPublisher;
        _formatter = formatter;
    }

    public async Task PublishAsync(
        Vacancy vacancy,
        CancellationToken cancellationToken)
    {
        string message = _formatter.Format(vacancy);

        await _channelPublisher.PublishAsync(
            message,
            cancellationToken);
    }
}