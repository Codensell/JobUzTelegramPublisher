using Telegram.Bot;

namespace JobVacancyBot.App.Telegram;

public sealed class TelegramChannelPublisher
{
    private readonly ITelegramBotClient _botClient;
    private readonly string _channelId;

    public TelegramChannelPublisher(
        ITelegramBotClient botClient,
        string channelId)
    {
        _botClient = botClient;
        _channelId = channelId;
    }

    public async Task PublishAsync(
    string message,
    CancellationToken cancellationToken)
{
    await _botClient.SendMessage(
        chatId: _channelId,
        text: message,
        cancellationToken: cancellationToken);
}
}