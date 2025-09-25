FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY services/AuditService/AuditService.csproj services/AuditService/
COPY Canonicals/Canonicals.csproj Canonicals/
RUN dotnet restore services/AuditService/AuditService.csproj
COPY . .
RUN dotnet publish services/AuditService/AuditService.csproj -c Release -o /app

# worker apps don't need ASP.NET runtime
FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "AuditService.dll"]
