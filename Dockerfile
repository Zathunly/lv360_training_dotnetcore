FROM mcr.microsoft.com/dotnet/sdk:9.0
WORKDIR /app

# Copy csproj files to leverage restore cache
COPY src/lv360_training.Domain/lv360_training.Domain.csproj ./lv360_training.Domain/
COPY src/lv360_training.Application/lv360_training.Application.csproj ./lv360_training.Application/
COPY src/lv360_training.Infrastructure/lv360_training.Infrastructure.csproj ./lv360_training.Infrastructure/
COPY src/lv360_training.Api/lv360_training.Api.csproj ./lv360_training.Api/

# Restore packages
RUN dotnet restore ./lv360_training.Api/lv360_training.Api.csproj

# Copy everything else (not strictly needed because of volume mount)
COPY src/ ./ 

WORKDIR /app/lv360_training.Api

CMD ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:8080"]
