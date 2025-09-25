FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY services/OrderService/OrderService.csproj services/OrderService/
COPY Canonicals/Canonicals.csproj Canonicals/
RUN dotnet restore services/OrderService/OrderService.csproj
COPY . .
RUN dotnet publish services/OrderService/OrderService.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "OrderService.dll"]
