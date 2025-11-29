FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ProductManagement.Web/ProductManagement.Web.csproj", "ProductManagement/"]
RUN dotnet restore "ProductManagement/ProductManagement.Web.csproj"
COPY . "./ProductManagement"
WORKDIR "/src/ProductManagement"
RUN dotnet build "./ProductManagement.Web/ProductManagement.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ProductManagement.Web/ProductManagement.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductManagement.Web.dll"]
