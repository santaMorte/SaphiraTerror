# Etapa 1 - build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore "SaphiraTerror.Web.csproj"
RUN dotnet publish "SaphiraTerror.Web.csproj" -c Release -o /app/publish

# Etapa 2 - runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "SaphiraTerror.Web.dll"]