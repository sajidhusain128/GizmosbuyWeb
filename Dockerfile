# For more info on HTTP files go to https://aka.ms/vs/httpfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8082

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY GizmosbuyWeb.sln .

# Copy csproj files for dependencies
COPY GizmosbuyWeb/Gizmosbuy.Web.csproj GizmosbuyWeb/
COPY Gizmosbuy.DAL/Gizmosbuy.DAL.csproj Gizmosbuy.DAL/
COPY Gizmosbuy.Core/Gizmosbuy.Core.csproj Gizmosbuy.Core/
COPY Gizmosbuy.BAL/Gizmosbuy.BAL.csproj Gizmosbuy.BAL/

RUN dotnet restore GizmosbuyWeb.sln
COPY . .
# WORKDIR "/src/GizmosbuyWeb"
RUN dotnet publish GizmosbuyWeb/Gizmosbuy.Web.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Gizmosbuy.Web.dll"]
