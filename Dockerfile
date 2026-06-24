FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["cartas_aifa/cartas_aifa.csproj", "cartas_aifa/"]
RUN dotnet restore "cartas_aifa/cartas_aifa.csproj"
COPY . .
WORKDIR "/src/cartas_aifa"
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}
ENTRYPOINT ["dotnet", "cartas_aifa.dll"]
