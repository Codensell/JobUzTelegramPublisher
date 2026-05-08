using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobVacancyBot.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddVacancyPublications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "vacancy_publications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VacancyId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ChannelId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vacancy_publications", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_vacancy_publications_VacancyId_ChannelKey",
                table: "vacancy_publications",
                columns: new[] { "VacancyId", "ChannelKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vacancy_publications");
        }
    }
}
