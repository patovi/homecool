FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/HomeCool.Api/HomeCool.Api.csproj HomeCool.Api/
RUN dotnet restore HomeCool.Api/HomeCool.Api.csproj

COPY src/HomeCool.Api/ HomeCool.Api/
RUN dotnet publish HomeCool.Api/HomeCool.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN mkdir -p /data && chmod 777 /data

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "HomeCool.Api.dll"]
