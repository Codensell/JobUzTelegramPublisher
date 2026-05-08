namespace JobVacancyBot.Application.Options;

public sealed class PublishingOptions
{
    public int MaxVacanciesPerRun { get; set; } = 10;

    public int MaxVacanciesToCollectPerRun { get; set; } = 100;
}
