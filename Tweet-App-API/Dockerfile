#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Tweet-App-API/Tweet-App-API.csproj", "Tweet-App-API/"]
RUN dotnet restore "Tweet-App-API/Tweet-App-API.csproj"
COPY . .
WORKDIR "/src/Tweet-App-API"
RUN dotnet build "Tweet-App-API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tweet-App-API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tweet-App-API.dll"]