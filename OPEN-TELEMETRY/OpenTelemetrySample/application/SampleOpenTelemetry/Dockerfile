FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src
COPY ["SampleOpenTelemetry.csproj", "./"]
RUN dotnet restore "SampleOpenTelemetry.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "SampleOpenTelemetry.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SampleOpenTelemetry.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SampleOpenTelemetry.dll"]
