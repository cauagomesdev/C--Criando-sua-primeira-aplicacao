# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy project manifest first to leverage layer cache.
# This layer is only invalidated when the .csproj changes.
COPY MeetupApi.Web/MeetupApi.Web.csproj MeetupApi.Web/
RUN dotnet restore MeetupApi.Web/MeetupApi.Web.csproj

COPY MeetupApi.Web/ MeetupApi.Web/
RUN dotnet publish MeetupApi.Web/MeetupApi.Web.csproj -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

# Non-root user shipped with the official .NET images
USER app

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "MeetupApi.Web.dll"]
