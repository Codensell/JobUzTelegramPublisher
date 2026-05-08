using System.Globalization;
using System.Net.Http.Json;
using JobVacancyBot.Application.Abstractions;
using JobVacancyBot.Application.Options;
using JobVacancyBot.Domain.Entities;
using JobVacancyBot.Infrastructure.HeadHunter.Dto;
using Microsoft.Extensions.Options;

namespace JobVacancyBot.Infrastructure.HeadHunter;

public sealed class HhVacancySource : IVacancySource
{
    private readonly HttpClient _httpClient;
    private readonly HeadHunterOptions _options;
    private readonly TelegramChannelsOptions _channelsOptions;

    public HhVacancySource(
        HttpClient httpClient,
        IOptions<HeadHunterOptions> options,
        IOptions<TelegramChannelsOptions> channelsOptions)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _channelsOptions = channelsOptions.Value;
    }

    public async Task<IReadOnlyList<Vacancy>> GetVacanciesAsync(CancellationToken cancellationToken)
    {
        Dictionary<string, Vacancy> vacancies = new(StringComparer.Ordinal);

        await CollectVacanciesAsync(
            searchText: null,
            pagesToScan: GetPositiveOrDefault(_options.MaxPagesToScan, 1),
            vacancies,
            cancellationToken);

        foreach (string searchQuery in GetChannelSearchQueries())
        {
            await CollectVacanciesAsync(
                searchQuery,
                GetPositiveOrDefault(_options.MaxPagesPerSearchQuery, 1),
                vacancies,
                cancellationToken);
        }

        return vacancies.Values
            .OrderByDescending(vacancy => vacancy.PublishedAt)
            .ToList();
    }

    private async Task CollectVacanciesAsync(
        string? searchText,
        int pagesToScan,
        Dictionary<string, Vacancy> vacancies,
        CancellationToken cancellationToken)
    {
        for (int page = 0; page < pagesToScan; page++)
        {
            string requestUri = BuildRequestUri(page, searchText);

            HttpResponseMessage httpResponse = await _httpClient.GetAsync(requestUri, cancellationToken);

            if (!httpResponse.IsSuccessStatusCode)
            {
                string errorBody = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

                throw new HttpRequestException(
                    $"HeadHunter API error for '{requestUri}': {(int)httpResponse.StatusCode} {httpResponse.ReasonPhrase}. Response: {errorBody}",
                    inner: null,
                    httpResponse.StatusCode);
            }

            HhVacanciesResponse? response = await httpResponse.Content
                .ReadFromJsonAsync<HhVacanciesResponse>(cancellationToken);

            if (response is null || response.Items.Length == 0)
            {
                break;
            }

            foreach (Vacancy vacancy in response.Items.Select(MapToVacancy))
            {
                vacancies.TryAdd(vacancy.ExternalId, vacancy);
            }

            if (response.Items.Length < _options.PerPage)
            {
                break;
            }
        }
    }

    private IEnumerable<string> GetChannelSearchQueries()
    {
        return _channelsOptions.Channels
            .SelectMany(channel => channel.SearchQueries)
            .Where(searchQuery => !string.IsNullOrWhiteSpace(searchQuery))
            .Select(searchQuery => searchQuery.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase);
    }

    private static int GetPositiveOrDefault(int value, int defaultValue)
    {
        return value > 0
            ? value
            : defaultValue;
    }

    private string BuildRequestUri(
        int page,
        string? searchText)
    {
        List<string> queryParameters =
        [
            $"area={Uri.EscapeDataString(_options.AreaId)}",
            $"period={_options.PeriodDays}",
            "order_by=publication_time",
            $"per_page={_options.PerPage}",
            $"page={page}"
        ];

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            queryParameters.Add($"text={Uri.EscapeDataString(searchText)}");
        }

        if (!string.IsNullOrWhiteSpace(_options.Host))
        {
            queryParameters.Add($"host={Uri.EscapeDataString(_options.Host)}");
        }

        foreach (string roleId in _options.ProfessionalRoleIds)
        {
            queryParameters.Add($"professional_role={Uri.EscapeDataString(roleId)}");
        }

        return $"vacancies?{string.Join("&", queryParameters)}";
    }

    private static Vacancy MapToVacancy(HhVacancyItem item)
    {
        return new Vacancy(
            externalId: item.Id,
            title: item.Name,
            companyName: item.Employer?.Name ?? "не указано",
            areaName: item.Area?.Name ?? "не указано",
            salaryFrom: item.Salary?.From,
            salaryTo: item.Salary?.To,
            currency: item.Salary?.Currency,
            experienceName: item.Experience?.Name ?? "не указано",
            employmentName: item.Employment?.Name ?? "не указано",
            scheduleName: item.Schedule?.Name ?? "не указано",
            requirementSnippet: item.Snippet?.Requirement,
            responsibilitySnippet: item.Snippet?.Responsibility,
            url: item.AlternateUrl,
            publishedAt: ParsePublishedAt(item.PublishedAt));
    }

    private static DateTime ParsePublishedAt(string publishedAt)
    {
        string normalizedPublishedAt = NormalizeTimeZoneOffset(publishedAt);

        if (DateTimeOffset.TryParse(
            normalizedPublishedAt,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal,
            out DateTimeOffset parsedPublishedAt))
        {
            return parsedPublishedAt.UtcDateTime;
        }

        throw new FormatException(
            $"Invalid HeadHunter published_at value: '{publishedAt}'.");
    }

    private static string NormalizeTimeZoneOffset(string value)
    {
        const int compactOffsetLength = 5;

        if (value.Length < compactOffsetLength)
        {
            return value;
        }

        int offsetStartIndex = value.Length - compactOffsetLength;
        char offsetSign = value[offsetStartIndex];

        if (offsetSign is not ('+' or '-'))
        {
            return value;
        }

        ReadOnlySpan<char> offsetDigits = value.AsSpan(offsetStartIndex + 1);

        foreach (char digit in offsetDigits)
        {
            if (!char.IsDigit(digit))
            {
                return value;
            }
        }

        return string.Concat(
            value.AsSpan(0, value.Length - 2),
            ":",
            value.AsSpan(value.Length - 2));
    }
}
