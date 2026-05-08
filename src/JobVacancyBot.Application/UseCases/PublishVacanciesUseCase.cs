using JobVacancyBot.Application.Abstractions;
using JobVacancyBot.Application.Options;
using JobVacancyBot.Domain.Entities;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.RegularExpressions;

namespace JobVacancyBot.Application.UseCases;

public sealed class PublishVacanciesUseCase
{
    private readonly IVacancyRepository _vacancyRepository;
    private readonly IVacancyPublisher _vacancyPublisher;
    private readonly PublishingOptions _options;
    private readonly TelegramChannelsOptions _channelsOptions;

    public PublishVacanciesUseCase(
        IVacancyRepository vacancyRepository,
        IVacancyPublisher vacancyPublisher,
        IOptions<PublishingOptions> options,
        IOptions<TelegramChannelsOptions> channelsOptions)
    {
        _vacancyRepository = vacancyRepository;
        _vacancyPublisher = vacancyPublisher;
        _options = options.Value;
        _channelsOptions = channelsOptions.Value;
    }

    public async Task<int> ExecuteAsync(CancellationToken cancellationToken)
    {
        int candidateLimit = Math.Max(_options.MaxVacanciesToCollectPerRun, 100);

        IReadOnlyList<Vacancy> vacancies = await _vacancyRepository.GetRecentAsync(
            candidateLimit,
            cancellationToken);

        int totalPublishedCount = 0;

        foreach (TelegramChannelOptions channel in _channelsOptions.Channels)
        {
            if (string.IsNullOrWhiteSpace(channel.Key) ||
                string.IsNullOrWhiteSpace(channel.ChannelId))
            {
                continue;
            }

            int channelPublishedCount = 0;

            foreach (Vacancy vacancy in vacancies)
            {
                if (channelPublishedCount >= _options.MaxVacanciesPerRun)
                {
                    break;
                }

                if (!IsChannelMatch(channel, vacancy))
                {
                    continue;
                }

                bool alreadyPublished = await _vacancyRepository.ExistsPublicationAsync(
                    vacancy.Id,
                    channel.Key,
                    cancellationToken);

                if (alreadyPublished)
                {
                    continue;
                }

                await _vacancyPublisher.PublishAsync(
                    vacancy,
                    channel.ChannelId,
                    cancellationToken);

                await _vacancyRepository.AddPublicationAsync(
                    new VacancyPublication(
                        vacancy.Id,
                        channel.Key,
                        channel.ChannelId,
                        DateTime.UtcNow),
                    cancellationToken);

                channelPublishedCount++;
                totalPublishedCount++;
            }

            Console.WriteLine($"Published to {channel.Key}: {channelPublishedCount}");
        }

        await _vacancyRepository.SaveChangesAsync(cancellationToken);

        return totalPublishedCount;
    }

    private static bool IsChannelMatch(
        TelegramChannelOptions channel,
        Vacancy vacancy)
    {
        string searchText = BuildSearchText(vacancy);

        return channel.Keywords.Any(keyword => IsKeywordMatch(searchText, keyword));
    }

    private static string BuildSearchText(Vacancy vacancy)
    {
        return WebUtility.HtmlDecode(string.Join(
            " ",
            vacancy.Title,
            vacancy.CompanyName,
            vacancy.ExperienceName,
            vacancy.EmploymentName,
            vacancy.ScheduleName,
            vacancy.RequirementSnippet,
            vacancy.ResponsibilitySnippet));
    }

    private static bool IsKeywordMatch(string searchText, string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return false;
        }

        string trimmedKeyword = keyword.Trim();

        bool canUseWordBoundary = trimmedKeyword.All(
            character => char.IsLetterOrDigit(character));

        if (!canUseWordBoundary)
        {
            return searchText.Contains(
                trimmedKeyword,
                StringComparison.OrdinalIgnoreCase);
        }

        string pattern = $@"(?<![\p{{L}}\p{{N}}]){Regex.Escape(trimmedKeyword)}(?![\p{{L}}\p{{N}}])";

        return Regex.IsMatch(
            searchText,
            pattern,
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    }
}
