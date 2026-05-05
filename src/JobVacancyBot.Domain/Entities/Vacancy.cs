namespace JobVacancyBot.Domain.Entities;

public sealed class Vacancy
{
    public Guid Id { get; private set; }

    public string ExternalId { get; private set; }
    public string Title { get; private set; }
    public string CompanyName { get; private set; }
    public string AreaName { get; private set; }

    public decimal? SalaryFrom { get; private set; }
    public decimal? SalaryTo { get; private set; }
    public string? Currency { get; private set; }

    public string ExperienceName { get; private set; }
    public string EmploymentName { get; private set; }
    public string ScheduleName { get; private set; }

    public string? RequirementSnippet { get; private set; }
    public string? ResponsibilitySnippet { get; private set; }

    public string Url { get; private set; }

    public DateTime PublishedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? PublishedToTelegramAt { get; private set; }

    private Vacancy()
    {
        ExternalId = string.Empty;
        Title = string.Empty;
        CompanyName = string.Empty;
        AreaName = string.Empty;
        ExperienceName = string.Empty;
        EmploymentName = string.Empty;
        ScheduleName = string.Empty;
        Url = string.Empty;
    }

    public Vacancy(
        string externalId,
        string title,
        string companyName,
        string areaName,
        decimal? salaryFrom,
        decimal? salaryTo,
        string? currency,
        string experienceName,
        string employmentName,
        string scheduleName,
        string? requirementSnippet,
        string? responsibilitySnippet,
        string url,
        DateTime publishedAt)
    {
        Id = Guid.NewGuid();
        ExternalId = externalId;
        Title = title;
        CompanyName = companyName;
        AreaName = areaName;
        SalaryFrom = salaryFrom;
        SalaryTo = salaryTo;
        Currency = currency;
        ExperienceName = experienceName;
        EmploymentName = employmentName;
        ScheduleName = scheduleName;
        RequirementSnippet = requirementSnippet;
        ResponsibilitySnippet = responsibilitySnippet;
        Url = url;
        PublishedAt = publishedAt;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkAsPublishedToTelegram(DateTime publishedAt)
    {
        PublishedToTelegramAt = publishedAt;
    }
}