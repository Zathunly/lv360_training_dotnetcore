FROM mcr.microsoft.com/dotnet/sdk:9.0
WORKDIR /app

# Copy only csproj files to leverage restore cache
COPY src/lv360_training.Domain/lv360_training.Domain.csproj ./lv360_training.Domain/
COPY src/lv360_training.Application/lv360_training.Application.csproj ./lv360_training.Application/
COPY src/lv360_training.Infrastructure/lv360_training.Infrastructure.csproj ./lv360_training.Infrastructure/
COPY src/lv360_training.Api/lv360_training.Api.csproj ./lv360_training.Api/

# Install dotnet-ef tool globally
RUN dotnet tool install --global dotnet-ef --version 9.*

# Add dotnet tools to PATH
ENV PATH="$PATH:/root/.dotnet/tools"

# Restore packages inside container
RUN dotnet restore ./lv360_training.Api/lv360_training.Api.csproj

# Copy the rest of the source code
COPY src/ ./ 

WORKDIR /app/lv360_training.Api

# Use watch run for hot reload
CMD ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:8080"]
