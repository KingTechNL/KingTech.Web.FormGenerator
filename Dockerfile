#Dockerfile for kingtech frontoffice image

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["KingTech.Web.FormGenerator.Example/KingTech.Web.FormGenerator.Example.csproj", "KingTech.Web.FormGenerator.Example/"]
RUN dotnet restore "KingTech.Web.FormGenerator.Example/KingTech.Web.FormGenerator.Example.csproj"
COPY  ["KingTech.Web.FormGenerator.Example/", "KingTech.Web.FormGenerator.Example/"]
WORKDIR "/src/KingTech.Web.FormGenerator.Example"
RUN dotnet build "KingTech.Web.FormGenerator.Example.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KingTech.Web.FormGenerator.Example.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KingTech.Web.FormGenerator.Example.dll"]