#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

cd "$SCRIPT_DIR"

echo "Starting PostgreSQL..."
docker compose up -d postgres

echo "Waiting for PostgreSQL..."
until docker compose exec -T postgres pg_isready -U postgres -d job_vacancy_bot >/dev/null 2>&1; do
  sleep 1
done

echo "Applying database migrations..."
dotnet ef database update \
  --project src/JobVacancyBot.Infrastructure/JobVacancyBot.Infrastructure.csproj \
  --startup-project src/JobVacancyBot.App/JobVacancyBot.App.csproj

echo "Building bot..."
dotnet build src/JobVacancyBot.App/JobVacancyBot.App.csproj \
  --no-restore \
  -m:1 \
  -nodeReuse:false \
  -p:UseSharedCompilation=false \
  -v:minimal

echo "Launching bot. Press Ctrl+C to stop."
dotnet src/JobVacancyBot.App/bin/Debug/net9.0/JobVacancyBot.App.dll
