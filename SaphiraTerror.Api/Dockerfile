FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .

# entra na pasta da API
WORKDIR /src/SaphiraTerror.Api

RUN dotnet restore "SaphiraTerror.Api.csproj"
RUN dotnet publish "SaphiraTerror.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "SaphiraTerror.Api.dll"]