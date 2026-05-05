using System.Text;
using JobVacancyBot.Domain.Entities;

namespace JobVacancyBot.App.Formatting;

public sealed class VacancyMessageFormatter
{
    public string Format(Vacancy vacancy)
    {
        var builder = new StringBuilder();

        builder.AppendLine($"💼 {FormatValue(vacancy.Title)}");
        builder.AppendLine();

        builder.AppendLine($"🏢 Компания: {FormatValue(vacancy.CompanyName)}");
        builder.AppendLine($"📍 Локация: {FormatValue(vacancy.AreaName)}");
        builder.AppendLine($"💰 Зарплата: {FormatSalary(vacancy)}");
        builder.AppendLine($"🧑‍💻 Опыт: {FormatValue(vacancy.ExperienceName)}");
        builder.AppendLine($"🕒 График: {FormatValue(vacancy.ScheduleName)}");
        builder.AppendLine($"📅 Опубликовано: {vacancy.PublishedAt:dd.MM.yyyy}");
        builder.AppendLine();

        builder.AppendLine("Требования:");
        builder.AppendLine(FormatValue(vacancy.RequirementSnippet));
        builder.AppendLine();

        builder.AppendLine("Обязанности:");
        builder.AppendLine(FormatValue(vacancy.ResponsibilitySnippet));
        builder.AppendLine();

        builder.AppendLine("Открыть вакансию:");
        builder.AppendLine(FormatValue(vacancy.Url));

        return builder.ToString();
    }

    private static string FormatSalary(Vacancy vacancy)
    {
        if (vacancy.SalaryFrom is null && vacancy.SalaryTo is null)
        {
            return "не указана";
        }

        if (vacancy.SalaryFrom is not null && vacancy.SalaryTo is not null)
        {
            return $"от {vacancy.SalaryFrom:N0} до {vacancy.SalaryTo:N0} {FormatValue(vacancy.Currency)}";
        }

        if (vacancy.SalaryFrom is not null)
        {
            return $"от {vacancy.SalaryFrom:N0} {FormatValue(vacancy.Currency)}";
        }

        return $"до {vacancy.SalaryTo:N0} {FormatValue(vacancy.Currency)}";
    }

    private static string FormatValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? "не указано"
            : value;
    }
}