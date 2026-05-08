using Telegram.Bot;

namespace JobVacancyBot.App.Telegram;

public sealed class TelegramChannelPublisher
{
    private readonly ITelegramBotClient _botClient;

    public TelegramChannelPublisher(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task PublishAsync(
        string channelId,
        string message,
        CancellationToken cancellationToken)
    {
        await _botClient.SendMessage(
            chatId: channelId,
            text: message,
            cancellationToken: cancellationToken);
    }
}
