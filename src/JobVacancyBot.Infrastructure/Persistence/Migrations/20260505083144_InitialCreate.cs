using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobVacancyBot.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "vacancies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    CompanyName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    AreaName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SalaryFrom = table.Column<decimal>(type: "numeric", nullable: true),
                    SalaryTo = table.Column<decimal>(type: "numeric", nullable: true),
                    Currency = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    ExperienceName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EmploymentName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ScheduleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RequirementSnippet = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ResponsibilitySnippet = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PublishedToTelegramAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vacancies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_vacancies_ExternalId",
                table: "vacancies",
                column: "ExternalId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vacancies");
        }
    }
}
