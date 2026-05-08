namespace JobVacancyBot.Domain.Entities;

public sealed class VacancyPublication
{
    public Guid Id { get; private set; }

    public Guid VacancyId { get; private set; }

    public string ChannelKey { get; private set; }

    public string ChannelId { get; private set; }

    public DateTime PublishedAt { get; private set; }

    private VacancyPublication()
    {
        ChannelKey = string.Empty;
        ChannelId = string.Empty;
    }

    public VacancyPublication(
        Guid vacancyId,
        string channelKey,
        string channelId,
        DateTime publishedAt)
    {
        Id = Guid.NewGuid();
        VacancyId = vacancyId;
        ChannelKey = channelKey;
        ChannelId = channelId;
        PublishedAt = publishedAt;
    }
}
