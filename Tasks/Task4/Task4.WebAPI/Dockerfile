FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Task4/Task4.WebAPI/Task4.WebAPI.csproj", "Task4/Task4.WebAPI/"]
COPY ["Task4/Task4.Implementation/Task4.Implementation.csproj", "Task4/Task4.Implementation/"]
RUN dotnet restore "Task4/Task4.WebAPI/Task4.WebAPI.csproj"
COPY . .
WORKDIR "/src/Task4/Task4.WebAPI"
RUN dotnet build "Task4.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Task4.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Task4.WebAPI.dll"]
