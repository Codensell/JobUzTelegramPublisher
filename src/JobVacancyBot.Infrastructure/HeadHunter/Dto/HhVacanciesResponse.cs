using System.Text.Json.Serialization;

namespace JobVacancyBot.Infrastructure.HeadHunter.Dto;

public sealed class HhVacanciesResponse
{
    [JsonPropertyName("items")]
    public HhVacancyItem[] Items { get; set; } = [];
}

public sealed class HhVacancyItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("employer")]
    public HhEmployer? Employer { get; set; }

    [JsonPropertyName("area")]
    public HhArea? Area { get; set; }

    [JsonPropertyName("salary")]
    public HhSalary? Salary { get; set; }

    [JsonPropertyName("experience")]
    public HhNamedValue? Experience { get; set; }

    [JsonPropertyName("employment")]
    public HhNamedValue? Employment { get; set; }

    [JsonPropertyName("schedule")]
    public HhNamedValue? Schedule { get; set; }

    [JsonPropertyName("snippet")]
    public HhSnippet? Snippet { get; set; }

    [JsonPropertyName("alternate_url")]
    public string AlternateUrl { get; set; } = string.Empty;

    [JsonPropertyName("published_at")]
    public string PublishedAt { get; set; } = string.Empty;
}

public sealed class HhEmployer
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public sealed class HhArea
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public sealed class HhSalary
{
    [JsonPropertyName("from")]
    public decimal? From { get; set; }

    [JsonPropertyName("to")]
    public decimal? To { get; set; }

    [JsonPropertyName("currency")]
    public string? Currency { get; set; }
}

public sealed class HhNamedValue
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public sealed class HhSnippet
{
    [JsonPropertyName("requirement")]
    public string? Requirement { get; set; }

    [JsonPropertyName("responsibility")]
    public string? Responsibility { get; set; }
}
