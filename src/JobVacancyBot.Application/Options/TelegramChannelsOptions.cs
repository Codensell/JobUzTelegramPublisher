namespace JobVacancyBot.Application.Options;

public sealed class TelegramChannelsOptions
{
    public TelegramChannelOptions[] Channels { get; set; } = [];
}

public sealed class TelegramChannelOptions
{
    public string Key { get; set; } = string.Empty;

    public string ChannelId { get; set; } = string.Empty;

    public string[] Keywords { get; set; } = [];

    public string[] SearchQueries { get; set; } = [];
}
