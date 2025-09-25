FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
# copy csproj first for better layer caching
COPY services/BookCatalogueService/BookCatalogueService.csproj services/BookCatalogueService/
COPY Canonicals/Canonicals.csproj Canonicals/
RUN dotnet restore services/BookCatalogueService/BookCatalogueService.csproj
# now copy the rest and publish
COPY . .
RUN dotnet publish services/BookCatalogueService/BookCatalogueService.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "BookCatalogueService.dll"]
