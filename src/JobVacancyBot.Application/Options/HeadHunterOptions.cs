namespace JobVacancyBot.Application.Options;

public sealed class HeadHunterOptions
{
    public string BaseUrl { get; set; } = string.Empty;

    public string AreaId { get; set; } = string.Empty;

    public string Host { get; set; } = string.Empty;

    public string[] ProfessionalRoleIds { get; set; } = [];

    public int PeriodDays { get; set; } = 1;

    public int PerPage { get; set; } = 10;

    public int MaxPagesToScan { get; set; } = 10;

    public int MaxPagesPerSearchQuery { get; set; } = 5;

    public string UserAgent { get; set; } = string.Empty;

    public string AccessToken { get; set; } = string.Empty;
}
