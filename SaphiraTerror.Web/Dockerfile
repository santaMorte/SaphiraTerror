FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .

RUN ls -la
RUN find . -maxdepth 3 -name "*.csproj" -print


RUN dotnet restore "SaphiraTerror.Web/SaphiraTerror.Web.csproj"
RUN dotnet publish "SaphiraTerror.Web/SaphiraTerror.Web.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "SaphiraTerror.Web.dll"]